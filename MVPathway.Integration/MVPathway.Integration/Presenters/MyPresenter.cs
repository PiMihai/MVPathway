using MVPathway.Logging.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using System.Threading.Tasks;

namespace MVPathway.Integration.Presenters
{
  class MyPresenter : BasePresenter
  {
    public MyPresenter(IDiContainer container, IViewModelManager viewModelManager, ILogger logger)
      : base(container, viewModelManager, logger)
    {
    }

    public override async Task<BaseViewModel> Show(BaseViewModel viewModel, object parameter)
    {
      return await base.Show(viewModel, parameter);
    }

    public override async Task<BaseViewModel> Close(BaseViewModel viewModel, object parameter)
    {
      return await base.Close(viewModel, parameter);
    }

    public override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText)
    {
      return await Task.FromResult(true);
    }
  }
}
