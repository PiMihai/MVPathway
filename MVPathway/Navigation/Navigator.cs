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

namespace MVPathway.Navigation
{
    public class Navigator : INavigator
    {
        private const string EXCEPTION_NULL_VM_INSTANCE = "Received ViewModel show request, but instance was null.";

        private const string ERROR_VM_STACK_EMPTY = "Received close request but VM stack is empty.";

        private string WARNING_VM_ALREADY_SHOWN(Type viewModelType) => $"Received show request for {viewModelType.Name}, but VM already shown.";
        private string WARNING_VM_STACK_ROOT_REACHED = "Received close request but already reached stack root.";

        private string INFO_ON_NAVIGATED_TO(Type viewModelType) => $"{viewModelType.Name} OnNavigatedTo called.";
        private string INFO_ON_NAVIGATING_FROM(Type viewModelType) => $"{viewModelType.Name} OnNavigatingFrom called.";

        private readonly IDiContainer _container;
        private readonly ILogger _logger;
        private readonly IViewModelManager _vmManager;
        private readonly IMessenger _messenger;

        private Stack<BaseViewModel> _navigationStack = new Stack<BaseViewModel>();

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

        public async Task ChangePresenter<TPresenter>()
            where TPresenter : IPresenter
        {
            await ChangePresenter(typeof(TPresenter));
        }

        public async Task ChangePresenter(Type presenterType)
        {
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
            await instance.Init();
            while (auxStack.Count > 0)
            {
                var nextVmToPush = auxStack.Pop();
                await Show(nextVmToPush);
            }
        }

        public async Task<TViewModel> Show<TViewModel>(object parameter = null)
                  where TViewModel : BaseViewModel
        {
            var viewModel = _container.Resolve<TViewModel>();
            return await Show(viewModel, parameter) as TViewModel;
        }

        public async Task<BaseViewModel> Show(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
        {
            var viewModel = _vmManager.ResolveViewModelByDefinition(definitionFilter);
            return await Show(viewModel, parameter);
        }

        public async Task<BaseViewModel> Show(BaseViewModel viewModel, object parameter = null)
        {
            var presenter = _container.Resolve<IPresenter>();

            if (viewModel == null)
            {
                throw new ArgumentNullException(EXCEPTION_NULL_VM_INSTANCE);
            }

            var viewModelType = viewModel.GetType();
            if (_navigationStack.Count > 0)
            {
                if (_navigationStack.Peek().GetType().Name == viewModelType.Name)
                {
                    _logger.LogWarning(WARNING_VM_ALREADY_SHOWN(viewModelType));
                    return null;
                }
                // actual UI work
                await presenter.OnClose(_navigationStack.Peek(),
                    _vmManager.ResolvePageForViewModel(_navigationStack.Peek()),
                    NavigationRequestType.FromShow);

                await callOnNavigatingFrom(_navigationStack.Peek(), null);
            }
            _navigationStack.Push(viewModel);
            _messenger.Send(new NavigationStackUpdatedMessage
            {
                ViewModel = viewModel
            });

            // actual UI work
            await presenter.OnShow(viewModel,
                _vmManager.ResolvePageForViewModel(viewModel),
                NavigationRequestType.FromShow);

            await callOnNavigatedTo(viewModel, parameter);
            return viewModel;
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
            var presenter = _container.Resolve<IPresenter>();

            if (viewModel == null)
            {
                throw new ArgumentNullException(EXCEPTION_NULL_VM_INSTANCE);
            }

            var resultTask = viewModel.ResultTask;
            await Show(viewModel, parameter);
            var result = await resultTask;
            await Close(parameter);
            return result;
        }

        public async Task<BaseViewModel> Close(object parameter = null)
        {
            var presenter = _container.Resolve<IPresenter>();

            if (_navigationStack.Count == 0)
            {
                throw new InvalidOperationException(ERROR_VM_STACK_EMPTY);
            }
            if (_navigationStack.Count == 1)
            {
                _logger.LogWarning(WARNING_VM_STACK_ROOT_REACHED);
                return null;
            }
            var currentViewModel = _navigationStack.Pop();
            _messenger.Send(new NavigationStackUpdatedMessage
            {
                WasPopped = true,
                ViewModel = currentViewModel
            });

            // actual UI work
            await presenter.OnClose(currentViewModel,
                _vmManager.ResolvePageForViewModel(currentViewModel),
                NavigationRequestType.FromClose);

            await callOnNavigatingFrom(currentViewModel, parameter);

            if (_navigationStack.Count > 0)
            {
                currentViewModel = _navigationStack.Peek();

                // actual UI work
                await presenter.OnShow(currentViewModel,
                    _vmManager.ResolvePageForViewModel(currentViewModel),
                    NavigationRequestType.FromClose);

                await callOnNavigatedTo(currentViewModel, null);
            }
            return currentViewModel;
        }

        public async Task DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
        {
            var presenter = _container.Resolve<IPresenter>();
            await presenter.OnDisplayAlert(title, message, okText, cancelText);
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
