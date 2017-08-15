using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;

namespace MVPathway.MVVM.Abstractions
{
    public interface IViewModelManager
    {
        void AutoScanAndRegister(Assembly assembly);
        ViewModelDefinition RegisterPageForViewModel<TViewModel, TPage>()
          where TViewModel : BaseViewModel
          where TPage : class;
        BaseViewModel ResolveViewModelByDefinition(Func<ViewModelDefinition, bool> definitionFilter);
        List<BaseViewModel> ResolveViewModelsByDefinition(Func<ViewModelDefinition, bool> definitionFilter);
        Page ResolvePageForViewModel<TViewModel>() where TViewModel : BaseViewModel;
        Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter);
        Page ResolvePageForViewModel(BaseViewModel viewModel);
        List<Page> ResolvePagesForViewModels(Func<ViewModelDefinition, bool> definitionFilter);
        ViewModelDefinition ResolveDefinitionForViewModel<TViewModel>() where TViewModel : BaseViewModel;
        ViewModelDefinition ResolveDefinitionForViewModel(BaseViewModel viewModel);
        List<Page> ResolvePagesForViewModels(List<BaseViewModel> viewModels);
    }
}