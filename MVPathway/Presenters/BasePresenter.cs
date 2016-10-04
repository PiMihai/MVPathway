using MVPathway.MVVM;
using System.Threading.Tasks;

namespace MVPathway.Presenters
{
  public abstract class BasePresenter
  {
    private const string cInvalidViewModelMessage = "TViewModel is not a valid ViewModel type.";

    protected IPathwayCore PathwayCore { get; private set; }

    public BasePresenter(IPathwayCore pathwayCore)
    {
      PathwayCore = pathwayCore;
    }

    internal async Task<TViewModel> Show<TViewModel>(object parameter)
      where TViewModel : BaseViewModel
    {
      var viewModel = PathwayCore.Resolve<TViewModel>();
      return await Show(viewModel, parameter);
    }

    internal async Task<TViewModel> Close<TViewModel>(object parameter)
      where TViewModel : BaseViewModel
    {
      var viewModel = PathwayCore.Resolve<TViewModel>();
      return await Close(viewModel, parameter);
    }

    protected internal virtual async Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter)
      where TViewModel : BaseViewModel
    {
      viewModel.OnNavigatedTo(parameter);
      return viewModel;
    }

    protected internal virtual async Task<TViewModel> Close<TViewModel>(TViewModel viewModel, object parameter)
      where TViewModel : BaseViewModel
    {
      viewModel.OnNavigatingFrom(parameter);
      return viewModel;
    }

    protected internal abstract Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText);
  }
}
