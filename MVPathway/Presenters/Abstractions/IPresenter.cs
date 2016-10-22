using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using System;
using System.Threading.Tasks;

namespace MVPathway.Presenters.Abstractions
{
  public interface IPresenter
  {
    Task<TViewModel> Show<TViewModel>(object parameter = null) where TViewModel : BaseViewModel;
    Task<BaseViewModel> Show(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null);
    Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter = null)
      where TViewModel : BaseViewModel;
    Task<TViewModel> Close<TViewModel>(object parameter = null) where TViewModel : BaseViewModel;
    Task<BaseViewModel> Close(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null);
    Task<TViewModel> Close<TViewModel>(TViewModel viewModel, object parameter = null)
      where TViewModel : BaseViewModel;
    Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText);
  }
}
