using System;
using Xamarin.Forms;

namespace MVPathway.MVVM.Abstractions
{
  public interface IViewModelManager
  {
    void RegisterPageForViewModel<TViewModel, TPage>(ViewModelDefinition definition = null)
      where TViewModel : BaseViewModel
      where TPage : class;
    Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter);
    Page ResolvePageForViewModel<TViewModel>(TViewModel viewModel) where TViewModel : BaseViewModel;
    BaseViewModel ResolveViewModel(Func<ViewModelDefinition, bool> definitionFilter);
  }
}