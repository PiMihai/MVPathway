using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using System.Threading.Tasks;
using System;
using MVPathway.Logging.Abstractions;
using System.Collections.Generic;

namespace MVPathway.Presenters
{
    public abstract class BasePresenter : IPresenter
    {
        private const string cInvalidViewModelMessage = "TViewModel is not a valid ViewModel type.";

        protected IDiContainer Container { get; private set; }
        protected IViewModelManager ViewModelManager { get; private set; }
        protected ILogger Logger { get; private set; }

        protected Stack<BaseViewModel> NavigationStack = new Stack<BaseViewModel>();

        protected BasePresenter(IDiContainer container, IViewModelManager viewModelManager, ILogger logger)
        {
            Container = container;
            ViewModelManager = viewModelManager;
            Logger = logger;
        }

        public async Task<TViewModel> Show<TViewModel>(object parameter = null)
          where TViewModel : BaseViewModel
        {
            var viewModel = Container.Resolve<TViewModel>();
            return await Show(viewModel, parameter) as TViewModel;
        }

        public async Task<BaseViewModel> Show(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
        {
            var viewModel = ViewModelManager.ResolveViewModel(definitionFilter);
            return await Show(viewModel, parameter);
        }

        public virtual async Task<BaseViewModel> Show(BaseViewModel viewModel, object parameter = null)
        {
            if (viewModel == null)
            {
                Logger.LogWarning("Received ViewModel show request, but instance was null.");
                return null;
            }

            var viewModelType = viewModel.GetType();
            if (NavigationStack.Count > 0)
            {
                if (NavigationStack.Peek().GetType().Name == viewModelType.Name)
                {
                    Logger.LogWarning($"Received show request for {viewModelType.Name}, but VM already shown.");
                    return null;
                }
                await callOnNavigatingFrom(NavigationStack.Peek(), null);
            }
            await callOnNavigatedTo(viewModel, parameter);
            NavigationStack.Push(viewModel);
            return viewModel;
        }

        public async Task<TResult> GetResult<TViewModel, TResult>(object parameter = null)
          where TViewModel : BaseResultViewModel<TResult>
        {
            var viewModel = Container.Resolve<TViewModel>();
            return await GetResult(viewModel, parameter);
        }

        public async Task<TResult> GetResult<TResult>(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
        {
            var viewModel = ViewModelManager.ResolveViewModel(definitionFilter) as BaseResultViewModel<TResult>;
            return await GetResult(viewModel, parameter);
        }

        public virtual async Task<TResult> GetResult<TResult>(BaseResultViewModel<TResult> viewModel, object parameter = null)
        {
            if (viewModel == null)
            {
                return default(TResult);
            }

            viewModel.TaskCompletionSource = new TaskCompletionSource<TResult>();
            await Show(viewModel, parameter).ConfigureAwait(false);
            var result = await viewModel.TaskCompletionSource.Task;
            await Close(viewModel, parameter);
            return result;
        }

        public async Task<TViewModel> Close<TViewModel>(object parameter = null)
          where TViewModel : BaseViewModel
        {
            var viewModel = Container.Resolve<TViewModel>();
            return await Close(viewModel, parameter) as TViewModel;
        }

        public async Task<BaseViewModel> Close(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
        {
            var viewModel = ViewModelManager.ResolveViewModel(definitionFilter);
            return await Close(viewModel, parameter);
        }

        public virtual async Task<BaseViewModel> Close(BaseViewModel viewModel, object parameter = null)
        {
            if(viewModel == null)
            {
                Logger.LogError("Received ViewModel close request, but instance was null.");
                return null;
            }

            var viewModelType = viewModel.GetType();
            if (NavigationStack.Count == 0)
            {
                Logger.LogError($"Received close request for {viewModelType.Name}, but VM stack is empty.");
                return null;
            }
            var stackTop = NavigationStack.Peek();
            if (stackTop.GetType() != viewModelType)
            {
                Logger.LogWarning($"Received close request for {viewModelType.Name}, but VM stack top type does not match. Close operation will still execute.");
            }
            await callOnNavigatingFrom(stackTop, parameter);
            NavigationStack.Pop();
            if (NavigationStack.Count > 0)
            {
                await callOnNavigatedTo(NavigationStack.Peek(), null);
            }
            return viewModel;
        }

        public abstract Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null);

        private async Task callOnNavigatedTo<TViewModel>(TViewModel viewModel, object parameter = null)
            where TViewModel : BaseViewModel
        {
            await viewModel.OnNavigatedTo(parameter);
            Logger.LogInfo($"{viewModel.GetType().Name} OnNavigatedTo called.");
        }

        private async Task callOnNavigatingFrom<TViewModel>(TViewModel viewModel, object parameter = null)
            where TViewModel : BaseViewModel
        {
            await viewModel.OnNavigatingFrom(parameter);
            Logger.LogInfo($"{viewModel.GetType().Name} OnNavigatingFrom called.");
        }
    }
}
