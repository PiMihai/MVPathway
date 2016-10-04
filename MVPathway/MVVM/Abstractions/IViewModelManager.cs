using System;
using MVPathway.MVVM.Abstractions;
using Xamarin.Forms;

namespace MVPathway.MVVM.Abstractions
{
  interface IViewModelManager
  {
    void RegisterPageForViewModel<TViewModel, TPage>(ViewModelDefinition definition)
      where TViewModel : BaseViewModel
      where TPage : class;
    Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter);
    Page ResolvePageForViewModel<TViewModel>() where TViewModel : BaseViewModel;
    BaseViewModel ResolveViewModel(Func<ViewModelDefinition, bool> definitionFilter);
  }
}