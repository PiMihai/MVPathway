using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using System.Threading.Tasks;
using System;
using MVPathway.Logging.Abstractions;
using System.Collections.Generic;
using Xamarin.Forms;
using MVPathway.Messages.Abstractions;
using MVPathway.Messages;

namespace MVPathway.Presenters
{
    public abstract class BasePresenter : IPresenter
    {
        private const string EXCEPTION_NULL_VM_INSTANCE = "Received ViewModel show request, but instance was null.";

        private const string ERROR_VM_STACK_EMPTY = "Received close request but VM stack is empty.";

        private string WARNING_VM_ALREADY_SHOWN(Type viewModelType) => $"Received show request for {viewModelType.Name}, but VM already shown.";
        private string WARNING_VM_STACK_ROOT_REACHED = "Received close request but already reached stack root.";

        private string INFO_ON_NAVIGATED_TO(Type viewModelType) => $"{viewModelType.Name} OnNavigatedTo called.";
        private string INFO_ON_NAVIGATING_FROM(Type viewModelType) => $"{viewModelType.Name} OnNavigatingFrom called.";

        protected IDiContainer Container { get; private set; }
        protected IViewModelManager VmManager { get; private set; }
        protected IMessagingManager Messenger { get; private set; }
        protected ILogger Logger { get; private set; }

        public Stack<BaseViewModel> NavigationStack { get; private set; }

        protected BasePresenter(IDiContainer container, IViewModelManager viewModelManager, IMessagingManager messenger, ILogger logger)
        {
            Container = container;
            VmManager = viewModelManager;
            Messenger = messenger;
            Logger = logger;
        }

        public virtual async Task Init()
        {
            NavigationStack = new Stack<BaseViewModel>();
        }

        public async Task<TViewModel> Show<TViewModel>(object parameter = null)
          where TViewModel : BaseViewModel
        {
            var viewModel = Container.Resolve<TViewModel>();
            return await Show(viewModel, parameter) as TViewModel;
        }

        public async Task<BaseViewModel> Show(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
        {
            var viewModel = VmManager.ResolveViewModelByDefinition(definitionFilter);
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
                    Logger.LogWarning(WARNING_VM_ALREADY_SHOWN(viewModelType));
                    return null;
                }
                // actual UI work
                await OnClose(NavigationStack.Peek(),
                    VmManager.ResolvePageForViewModel(NavigationStack.Peek()), NavigationRequestType.FromShow);
                await callOnNavigatingFrom(NavigationStack.Peek(), null);
            }
            NavigationStack.Push(viewModel);
            Messenger.Send(new NavigationStackUpdatedMessage());

            // actual UI work
            await OnShow(viewModel, VmManager.ResolvePageForViewModel(viewModel),
                NavigationRequestType.FromShow);
            await callOnNavigatedTo(viewModel, parameter);
            return viewModel;
        }

        public async Task<ViewModelResult<TResult>> GetResult<TViewModel, TResult>(object parameter = null)
          where TViewModel : BaseResultViewModel<TResult>
        {
            var viewModel = Container.Resolve<TViewModel>();
            return await GetResult(viewModel, parameter);
        }

        public async Task<ViewModelResult<TResult>> GetResult<TResult>(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
        {
            var viewModel = VmManager.ResolveViewModelByDefinition(definitionFilter) as BaseResultViewModel<TResult>;
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
                Logger.LogWarning(WARNING_VM_STACK_ROOT_REACHED);
                return null;
            }
            var currentViewModel = NavigationStack.Pop();
            Messenger.Send(new NavigationStackUpdatedMessage());

            // actual UI work
            await OnClose(currentViewModel, VmManager.ResolvePageForViewModel(currentViewModel),
                NavigationRequestType.FromClose);
            await callOnNavigatingFrom(currentViewModel, parameter);

            if (NavigationStack.Count > 0)
            {
                currentViewModel = NavigationStack.Peek();

                // actual UI work
                await OnShow(currentViewModel, VmManager.ResolvePageForViewModel(currentViewModel),
                    NavigationRequestType.FromClose);
                await callOnNavigatedTo(currentViewModel, null);
            }
            return currentViewModel;
        }

        protected abstract Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType);
        protected abstract Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType);

        public abstract Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null);

        private async Task callOnNavigatedTo<TViewModel>(TViewModel viewModel, object parameter = null)
            where TViewModel : BaseViewModel
        {
            await viewModel.OnNavigatedTo(parameter);
            Logger.LogInfo(INFO_ON_NAVIGATED_TO(viewModel.GetType()));
        }

        private async Task callOnNavigatingFrom<TViewModel>(TViewModel viewModel, object parameter = null)
            where TViewModel : BaseViewModel
        {
            await viewModel.OnNavigatingFrom(parameter);
            Logger.LogInfo(INFO_ON_NAVIGATING_FROM(viewModel.GetType()));
        }
    }
}
