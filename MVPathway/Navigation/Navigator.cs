using System;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using MVPathway.MVVM.Abstractions;
using System.Threading.Tasks;
using MVPathway.MVVM;
using MVPathway.Messages.Abstractions;
using MVPathway.Logging.Abstractions;
using System.Collections.Generic;
using MVPathway.Messages;
using System.Linq;

namespace MVPathway.Navigation
{
    public class Navigator : INavigator
    {
        private const string ERROR_NULL_VM_INSTANCE = "Received ViewModel show request but instance was null.";
        private const string ERROR_SHOW_NO_VM_SATISFIED_PREDICATE = "Received ViewModel show by definition request but no ViewModel definition satisfied predicate.";
        private string ERROR_NULL_TYPED_VM_INSTANCE(Type viewModelType) => $"Received show request for {viewModelType.Name} but instance was null.";

        private const string ERROR_VM_STACK_EMPTY = "Received close request but VM stack is empty.";

        private string WARNING_VM_ALREADY_SHOWN(Type viewModelType) => $"Received show request for {viewModelType.Name} but VM already shown. Navigator will go back to that ViewModel.";
        private string WARNING_VM_STACK_ROOT_REACHED = "Received close request but already reached stack root.";

        private string INFO_ON_NAVIGATED_TO(Type viewModelType) => $"{viewModelType.Name} ONT called.";
        private string INFO_ON_NAVIGATING_FROM(Type viewModelType) => $"{viewModelType.Name} ONF called.";

        private readonly IDiContainer _container;
        private readonly ILogger _logger;
        private readonly IViewModelManager _vmManager;
        private readonly IMessenger _messenger;

        private Stack<BaseViewModel> _navigationStack = new Stack<BaseViewModel>();

        public bool DuringRequestedTransition { get; private set; }

        public Navigator(IDiContainer container,
                         ILogger logger,
                         IViewModelManager vmManager,
                         IMessenger messenger)
        {
            _container = container;
            _logger = logger;
            _vmManager = vmManager;
            _messenger = messenger;
        }

        public async Task ChangePresenter<TPresenter>(Action<TPresenter> configure = null)
            where TPresenter : class, IPresenter
        {
            await ChangePresenter(typeof(TPresenter), p => configure?.Invoke(p as TPresenter));
        }

        public async Task ChangePresenter(Type presenterType, Action<IPresenter> configure = null)
        {
            DuringRequestedTransition = true;
            var auxStack = new Stack<BaseViewModel>();
            while (_navigationStack.Count > 0)
            {
                var viewModel = _navigationStack.Pop();
                auxStack.Push(viewModel);
                _messenger.Send(new NavigationStackUpdatedMessage
                {
                    ViewModel = viewModel,
                    WasPopped = true
                });
            }
            _container.Register(presenterType);
            var instance = _container.Resolve(presenterType) as IPresenter;
            _container.RegisterInstance(instance);
            configure?.Invoke(instance);
            await instance.Init();
            while (auxStack.Count > 0)
            {
                var nextVmToPush = auxStack.Pop();
                await Show(nextVmToPush);
            }
            DuringRequestedTransition = false;
        }

        public async Task Show<TViewModel>(object parameter = null)
                  where TViewModel : BaseViewModel
        {
            var viewModel = _container.Resolve<TViewModel>();
            if (viewModel == null)
            {
                _logger.LogError(ERROR_NULL_TYPED_VM_INSTANCE(typeof(TViewModel)));
                return;
            }
            await Show(viewModel, parameter);
        }

        public async Task Show(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
        {
            var viewModel = _vmManager.ResolveViewModelByDefinition(definitionFilter);
            if (viewModel == null)
            {
                _logger.LogError(ERROR_SHOW_NO_VM_SATISFIED_PREDICATE);
                return;
            }
            await Show(viewModel, parameter);
        }

        public async Task Show(BaseViewModel viewModel, object parameter = null)
        {
            if (viewModel == null)
            {
                _logger.LogError(ERROR_NULL_VM_INSTANCE);
                return;
            }

            var presenter = _container.Resolve<IPresenter>();

            var viewModelType = viewModel.GetType();
            if (_navigationStack.Count > 0)
            {
                if (_navigationStack.Any(vm => vm.GetType().Name == viewModelType.Name))
                {
                    _logger.LogWarning(WARNING_VM_ALREADY_SHOWN(viewModelType));
                    while (_navigationStack.Peek().GetType().Name != viewModelType.Name)
                    {
                        await Close();
                    }
                    return;
                }

                DuringRequestedTransition = true;
                await presenter.OnClose(_navigationStack.Peek(),
                    _vmManager.ResolvePageForViewModel(_navigationStack.Peek()),
                    NavigationRequestType.FromShow);
                DuringRequestedTransition = false;

                await callOnNavigatingFrom(_navigationStack.Peek());
            }
            _navigationStack.Push(viewModel);
            _messenger.Send(new NavigationStackUpdatedMessage
            {
                ViewModel = viewModel
            });

            DuringRequestedTransition = true;
            await presenter.OnShow(viewModel,
                _vmManager.ResolvePageForViewModel(viewModel),
                NavigationRequestType.FromShow);
            DuringRequestedTransition = false;

            await callOnNavigatedTo(viewModel, parameter);
        }

        public async Task<ViewModelResult<TResult>> GetResult<TViewModel, TResult>(object parameter = null)
            where TViewModel : BaseResultViewModel<TResult>
        {
            var viewModel = _container.Resolve<TViewModel>();
            return await GetResult(viewModel, parameter);
        }

        public async Task<ViewModelResult<TResult>> GetResult<TResult>(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
        {
            var viewModel = _vmManager.ResolveViewModelByDefinition(definitionFilter) as BaseResultViewModel<TResult>;
            return await GetResult(viewModel, parameter);
        }

        public async Task<ViewModelResult<TResult>> GetResult<TResult>(BaseResultViewModel<TResult> viewModel, object parameter = null)
        {
            if (viewModel == null)
            {
                _logger.LogError(ERROR_NULL_VM_INSTANCE);
            }

            var resultTask = viewModel.ResultTask;
            await Show(viewModel, parameter);
            var result = await resultTask;
            await Close(parameter);
            return result;
        }

        public async Task Close(object parameter = null)
        {
            var presenter = _container.Resolve<IPresenter>();

            if (_navigationStack.Count == 0)
            {
                throw new InvalidOperationException(ERROR_VM_STACK_EMPTY);
            }
            if (_navigationStack.Count == 1)
            {
                _logger.LogWarning(WARNING_VM_STACK_ROOT_REACHED);
                return;
            }
            var currentViewModel = _navigationStack.Pop();
            _messenger.Send(new NavigationStackUpdatedMessage
            {
                WasPopped = true,
                ViewModel = currentViewModel
            });

            DuringRequestedTransition = true;
            await presenter.OnClose(currentViewModel,
                _vmManager.ResolvePageForViewModel(currentViewModel),
                NavigationRequestType.FromClose);
            DuringRequestedTransition = false;

            await callOnNavigatingFrom(currentViewModel, parameter);

            if (_navigationStack.Count > 0)
            {
                currentViewModel = _navigationStack.Peek();

                DuringRequestedTransition = true;
                await presenter.OnShow(currentViewModel,
                    _vmManager.ResolvePageForViewModel(currentViewModel),
                    NavigationRequestType.FromClose);
                DuringRequestedTransition = false;

                await callOnNavigatedTo(currentViewModel);
            }
        }

        public async Task DisplayAlertAsync(string title, string message, string okText)
        {
            var presenter = _container.Resolve<IPresenter>();
            await presenter.OnDisplayAlert(title, message, okText);
        }

        public async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText)
        {
            var presenter = _container.Resolve<IPresenter>();
            return await presenter.OnDisplayAlert(title, message, okText, cancelText);
        }

        private async Task callOnNavigatedTo<TViewModel>(TViewModel viewModel, object parameter = null)
            where TViewModel : BaseViewModel
        {
            await viewModel.OnNavigatedTo(parameter);
            _logger.LogInfo(INFO_ON_NAVIGATED_TO(viewModel.GetType()));
        }

        private async Task callOnNavigatingFrom<TViewModel>(TViewModel viewModel, object parameter = null)
            where TViewModel : BaseViewModel
        {
            await viewModel.OnNavigatingFrom(parameter);
            _logger.LogInfo(INFO_ON_NAVIGATING_FROM(viewModel.GetType()));
        }
    }
}
