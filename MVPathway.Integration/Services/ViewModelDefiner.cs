using MVPathway.Integration.Services.Contracts;
using MVPathway.Integration.ViewModels;
using MVPathway.MVVM.Abstractions;
using MVPathway.Utils.Presenters;
using MVPathway.Utils.ViewModels.Qualities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Integration.Services
{
    public class ViewModelDefiner : IViewModelDefiner
    {
        private readonly IViewModelManager _vmManager;

        private readonly Dictionary<Type, ViewModelDefinition>
            _vmDefs = new Dictionary<Type, ViewModelDefinition>();

        public ViewModelDefiner(IViewModelManager vmManager)
        {
            _vmManager = vmManager;
        }

        public void Init()
        {
            _vmDefs[typeof(AViewModel)] = _vmManager.ResolveDefinitionForViewModel<AViewModel>();
            _vmDefs[typeof(BViewModel)] = _vmManager.ResolveDefinitionForViewModel<BViewModel>();
            _vmDefs[typeof(CViewModel)] = _vmManager.ResolveDefinitionForViewModel<CViewModel>();
            _vmDefs[typeof(DViewModel)] = _vmManager.ResolveDefinitionForViewModel<DViewModel>();
            _vmDefs[typeof(EViewModel)] = _vmManager.ResolveDefinitionForViewModel<EViewModel>();
            _vmDefs[typeof(FViewModel)] = _vmManager.ResolveDefinitionForViewModel<FViewModel>();
            _vmDefs[typeof(GViewModel)] = _vmManager.ResolveDefinitionForViewModel<GViewModel>();
            _vmDefs[typeof(HViewModel)] = _vmManager.ResolveDefinitionForViewModel<HViewModel>();
        }

        public async Task RedefineBasedOnPresenterType(Type presenterType)
        {
            await ensureNoPageHasParent();

            _vmDefs[typeof(AViewModel)].ClearQualities();
            _vmDefs[typeof(BViewModel)].ClearQualities();
            _vmDefs[typeof(CViewModel)].ClearQualities();
            _vmDefs[typeof(DViewModel)].ClearQualities();
            _vmDefs[typeof(EViewModel)].ClearQualities();
            _vmDefs[typeof(FViewModel)].ClearQualities();
            _vmDefs[typeof(GViewModel)].ClearQualities();
            _vmDefs[typeof(HViewModel)].ClearQualities();

            if (presenterType == typeof(MasterDetailPresenter))
            {
                _vmDefs[typeof(AViewModel)].AddQuality<IParentQuality>();

                _vmDefs[typeof(BViewModel)].AddQuality<IMainChildQuality>();

                _vmDefs[typeof(CViewModel)].AddQuality<IChildQuality>();

                _vmDefs[typeof(DViewModel)].AddQuality<IChildQuality>();
                _vmDefs[typeof(DViewModel)].AddQuality<IFullscreenQuality>();

                _vmDefs[typeof(EViewModel)].AddQuality<IModalQuality>();

                _vmDefs[typeof(FViewModel)].AddQuality<IModalQuality>();

                _vmDefs[typeof(GViewModel)].AddQuality<IModalQuality>();
                _vmDefs[typeof(GViewModel)].AddQuality<IFullscreenQuality>();

                _vmDefs[typeof(HViewModel)].AddQuality<IModalQuality>();
                _vmDefs[typeof(HViewModel)].AddQuality<IFullscreenQuality>();
            }
            else if (presenterType == typeof(TabbedPresenter))
            {
                _vmDefs[typeof(AViewModel)].AddQuality<IModalQuality>();
                _vmDefs[typeof(BViewModel)].AddQuality<IChildQuality>();
                _vmDefs[typeof(CViewModel)].AddQuality<IModalQuality>();
                _vmDefs[typeof(DViewModel)].AddQuality<IChildQuality>();
                _vmDefs[typeof(EViewModel)].AddQuality<IModalQuality>();
                _vmDefs[typeof(FViewModel)].AddQuality<IChildQuality>();
                _vmDefs[typeof(GViewModel)].AddQuality<IModalQuality>();
                _vmDefs[typeof(HViewModel)].AddQuality<IChildQuality>();
            }
        }

        private async Task ensureNoPageHasParent()
        {
            if (Application.Current.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.PopToRootAsync();
                var oldPage = navigationPage.CurrentPage;
                await navigationPage.PushAsync(new ContentPage());
                navigationPage.Navigation.RemovePage(oldPage);
            }
            Application.Current.MainPage = new ContentPage();
        }
    }
}
