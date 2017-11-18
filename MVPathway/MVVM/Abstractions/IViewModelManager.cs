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
        ViewModelDefinition RegisterPageForViewModel(Type viewModelType, Type pageType);
        BaseViewModel ResolveViewModelByDefinition(Func<ViewModelDefinition, bool> definitionFilter);
        IEnumerable<BaseViewModel> ResolveViewModelsByDefinition(Func<ViewModelDefinition, bool> definitionFilter);
        Page ResolvePageForViewModel<TViewModel>() where TViewModel : BaseViewModel;
        Page ResolvePageForViewModel(Type viewModelType);
        Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter);
        Page ResolvePageForViewModel(BaseViewModel viewModel);
        IEnumerable<Page> ResolvePagesForViewModels(Func<ViewModelDefinition, bool> definitionFilter);
        ViewModelDefinition ResolveDefinitionForViewModel<TViewModel>() where TViewModel : BaseViewModel;
        List<Page> ResolvePagesForViewModels(List<BaseViewModel> viewModels);
    }
}