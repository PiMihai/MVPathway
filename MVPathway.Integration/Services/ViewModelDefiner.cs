using MVPathway.Integration.Pages;
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
        private readonly IDiContainer _container;
        private readonly IViewModelManager _vmManager;

        private Dictionary<Type, ViewModelDefinition> _vmDefs = new Dictionary<Type, ViewModelDefinition>();

        public ViewModelDefiner(IDiContainer container, IViewModelManager vmManager)
        {
            _container = container;
            _vmManager = vmManager;
        }

        public void DefineInitial()
        {
            _vmDefs[typeof(AViewModel)] = _vmManager.RegisterPageForViewModel<AViewModel, APage>();
            _vmDefs[typeof(BViewModel)] = _vmManager.RegisterPageForViewModel<BViewModel, BPage>();
            _vmDefs[typeof(CViewModel)] = _vmManager.RegisterPageForViewModel<CViewModel, CPage>();
            _vmDefs[typeof(DViewModel)] = _vmManager.RegisterPageForViewModel<DViewModel, DPage>();
            _vmDefs[typeof(EViewModel)] = _vmManager.RegisterPageForViewModel<EViewModel, EPage>();
            _vmDefs[typeof(FViewModel)] = _vmManager.RegisterPageForViewModel<FViewModel, FPage>();
            _vmDefs[typeof(GViewModel)] = _vmManager.RegisterPageForViewModel<GViewModel, GPage>();
            _vmDefs[typeof(HViewModel)] = _vmManager.RegisterPageForViewModel<HViewModel, HPage>();
        }

        public async Task RedefineBasedOnPresenterType(Type presenterType)
        {
            await ensureNoPageHasParent();

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
