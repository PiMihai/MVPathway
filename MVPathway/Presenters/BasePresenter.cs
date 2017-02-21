using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using System.Threading.Tasks;
using System;

namespace MVPathway.Presenters
{
  public abstract class BasePresenter : IPresenter
  {
    private const string cInvalidViewModelMessage = "TViewModel is not a valid ViewModel type.";

    protected IDiContainer Container { get; private set; }
    protected IViewModelManager ViewModelManager { get; private set; }

    protected BasePresenter(IDiContainer container, IViewModelManager viewModelManager)
    {
      Container = container;
      ViewModelManager = viewModelManager;
    }

    public async Task<TViewModel> Show<TViewModel>(object parameter = null)
      where TViewModel : BaseViewModel
    {
      var viewModel = Container.Resolve<TViewModel>();
      return await Show(viewModel, parameter);
    }

    public async Task<BaseViewModel> Show(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
    {
      var viewModel = ViewModelManager.ResolveViewModel(definitionFilter);
      if (ViewModelManager == null)
      {
        return null;
      }
      return await Show(viewModel, parameter);
    }

    public virtual async Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter = null)
      where TViewModel : BaseViewModel
    {
      await viewModel.OnNavigatedTo(parameter);
      return viewModel;
    }

    public async Task<TResult> GetResult<TViewModel, TResult>(object parameter = null) where TViewModel : BaseResultViewModel<TResult>
    {
      var viewModel = Container.Resolve<TViewModel>();
      return await GetResult(viewModel, parameter);
    }

    public async Task<TResult> GetResult<TResult>(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
    {
      var viewModel = ViewModelManager.ResolveViewModel(definitionFilter) as BaseResultViewModel<TResult>;
      if (ViewModelManager == null)
      {
        return default(TResult);
      }
      return await GetResult(viewModel, parameter);
    }

    public virtual async Task<TResult> GetResult<TResult>(BaseResultViewModel<TResult> viewModel, object parameter = null)
    {
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
      return await Close(viewModel, parameter);
    }

    public async Task<BaseViewModel> Close(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null)
    {
      var viewModel = ViewModelManager.ResolveViewModel(definitionFilter);
      if (ViewModelManager == null)
      {
        return null;
      }
      return await Close(viewModel, parameter);
    }

    public virtual async Task<TViewModel> Close<TViewModel>(TViewModel viewModel, object parameter = null)
      where TViewModel : BaseViewModel
    {
      await viewModel.OnNavigatingFrom(parameter);
      return viewModel;
    }

    public abstract Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null);
  }
}
