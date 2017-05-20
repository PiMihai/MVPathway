using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MVPathway.MVVM.Abstractions
{
    public interface IViewModelManager
    {
        ViewModelDefinition RegisterPageForViewModel<TViewModel, TPage>()
          where TViewModel : BaseViewModel
          where TPage : class;
        BaseViewModel ResolveViewModelByDefinition(Func<ViewModelDefinition, bool> definitionFilter);
        List<BaseViewModel> ResolveViewModelsByDefinition(Func<ViewModelDefinition, bool> definitionFilter);
        Page ResolvePageForViewModel<TViewModel>() where TViewModel : BaseViewModel;
        Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter);
        Page ResolvePageForViewModel(BaseViewModel viewModel);
    }
}