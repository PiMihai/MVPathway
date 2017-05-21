using MVPathway.Logging.Abstractions;
using MVPathway.Messages;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters;
using MVPathway.Presenters.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVPathway.MVVM
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
        private readonly IViewModelManager _vmManager;
        private readonly INavigationBus _navigationBus;
        private readonly IMessenger _messenger;
        private readonly ILogger _logger;

        public Stack<BaseViewModel> NavigationStack { get; private set; } = new Stack<BaseViewModel>();

        protected Navigator(IDiContainer container,
                            IViewModelManager viewModelManager,
                            INavigationBus navigationBus,
                            IMessenger messenger,
                            ILogger logger)
        {
            _container = container;
            _vmManager = viewModelManager;
            _navigationBus = navigationBus;
            _messenger = messenger;
            _logger = logger;

            _navigationBus.ShowRequested += async (s, e) =>
            {
                if (!(s is IPresenter) || e.ViewModel == null)
                {
                    return;
                }
                await Show(e.ViewModel);
            };
            _navigationBus.CloseRequested += async (s, e) =>
            {
                if (!(s is IPresenter))
                {
                    return;
                }
                await Close();
            };
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
            if (viewModel == null)
            {
                throw new ArgumentNullException(EXCEPTION_NULL_VM_INSTANCE);
            }

            var viewModelType = viewModel.GetType();
            if (NavigationStack.Count > 0)
            {
                if (NavigationStack.Peek().GetType().Name == viewModelType.Name)
                {
                    _logger.LogWarning(WARNING_VM_ALREADY_SHOWN(viewModelType));
                    return null;
                }
                // actual UI work
                var closeEvent = new NavigationBusNavigateEventArgs
                {
                    ViewModel = NavigationStack.Peek(),
                    Page = _vmManager.ResolvePageForViewModel(NavigationStack.Peek()),
                    RequestType = NavigationRequestType.FromShow
                };
                _navigationBus.SendClose(this, closeEvent);
                await callOnNavigatingFrom(NavigationStack.Peek(), null);
            }
            NavigationStack.Push(viewModel);
            _messenger.Send(new NavigationStackUpdatedMessage());

            // actual UI work
            var showEvent = new NavigationBusNavigateEventArgs
            {
                ViewModel = viewModel,
                Page = _vmManager.ResolvePageForViewModel(viewModel),
                RequestType = NavigationRequestType.FromShow
            };
            _navigationBus.SendShow(this, showEvent);
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
            if (NavigationStack.Count == 0)
            {
                throw new InvalidOperationException(ERROR_VM_STACK_EMPTY);
            }
            if (NavigationStack.Count == 1)
            {
                _logger.LogWarning(WARNING_VM_STACK_ROOT_REACHED);
                return null;
            }
            var currentViewModel = NavigationStack.Pop();
            _messenger.Send(new NavigationStackUpdatedMessage());

            // actual UI work
            var closeEvent = new NavigationBusNavigateEventArgs
            {
                ViewModel = currentViewModel,
                Page = _vmManager.ResolvePageForViewModel(currentViewModel),
                RequestType = NavigationRequestType.FromClose
            };
            _navigationBus.SendClose(this, closeEvent);
            await callOnNavigatingFrom(currentViewModel, parameter);

            if (NavigationStack.Count > 0)
            {
                currentViewModel = NavigationStack.Peek();

                // actual UI work
                var showEvent = new NavigationBusNavigateEventArgs
                {
                    ViewModel = currentViewModel,
                    Page = _vmManager.ResolvePageForViewModel(currentViewModel),
                    RequestType = NavigationRequestType.FromClose
                };
                _navigationBus.SendShow(this, showEvent);
                await callOnNavigatedTo(currentViewModel, null);
            }
            return currentViewModel;
        }

        public void DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
            => _navigationBus.SendAlert(this, new NavigationBusAlertEventArgs
            {
                Title = title,
                Message = message,
                OkText = okText,
                CancelText = cancelText
            });

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
