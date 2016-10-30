using MVPathway.Logging.Abstractions;
using MVPathway.MVVM.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenters
{
  public class SinglePagePresenter : BasePresenter
  {
    private readonly ILogger mLogger;

    public SinglePagePresenter(ILogger logger, IViewModelManager viewModelManager, IDiContainer container)
      : base(container, viewModelManager)
    {
      mLogger = logger;
    }

    public override async Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter)
    {
      await base.Show(viewModel, parameter);
      try
      {
        var page = ViewModelManager.ResolvePageForViewModel(viewModel);
        Application.Current.MainPage = page;
      }
      catch
      {
        mLogger.LogError($"Page not registered for ViewModel type {typeof(TViewModel).FullName}");
        return null;
      }
      return viewModel;
    }

    public override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
    {
      if (cancelText != null)
      {
        return await Application.Current.MainPage?.DisplayAlert(title, message, okText, cancelText);
      }
      await Application.Current.MainPage?.DisplayAlert(title, message, okText);
      return true;
    }
  }
}
