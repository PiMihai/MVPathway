using MVPathway.MVVM.Abstractions;
using System;
using System.Threading.Tasks;
using MVPathway.Integration.ViewModels;
using MVPathway.Integration.ViewModels.Qualities;
using MVPathway.Integration.Tasks.Base;
using MVPathway.Utils.ViewModels.Qualities;
using MVPathway.Integration.Pages;
using MVPathway.Presenters.Abstractions;

namespace MVPathway.Integration.Tasks.Core
{
    public class PresenterDelegationTask : CoreIntegrationTask
    {
        private readonly IDiContainer _container;
        private readonly IViewModelManager _viewModelManager;
        private IPresenter _presenter;

        public PresenterDelegationTask(IDiContainer container, IViewModelManager viewModelManager)
        {
            _container = container;
            _viewModelManager = viewModelManager;
        }

        public override async Task<bool> Execute()
        {
            _presenter = _container.Resolve<IPresenter>();
            const string cPageException = "Cannot create page";

            var menuVmDef = new ViewModelDefinition();
            menuVmDef.AddQuality<MenuQuality>();
            var mainChildVmDef = new ViewModelDefinition();
            mainChildVmDef.AddQuality<MainChildQuality>();
            mainChildVmDef.AddQuality<MyQuality>();

            try
            {
                _viewModelManager.RegisterPageForViewModel<FirstViewModel, FirstPage>(menuVmDef);
                _viewModelManager.RegisterPageForViewModel<SecondViewModel, SecondPage>(mainChildVmDef);
            }
            catch (Exception e) when (e.Message.StartsWith(cPageException)) { }

            // show
            var first = _presenter.Show<FirstViewModel>().Result;
            if (first == null || !first.NavTo)
            {
                return false;
            }
            var second = await _presenter.Show(
              def => def.HasQuality<MyQuality>()) as SecondViewModel;
            if (second == null || !second.NavTo)
            {
                return false;
            }

            // close
            second = await _presenter.Close(
              x => x.HasQuality<MyQuality>()) as SecondViewModel;
            if (second == null || !second.NavFrom)
            {
                return false;
            }
            first = await _presenter.Close<FirstViewModel>();

            return first != null && first.NavFrom;

            // TODO : find way to test alerts
            //return PathwayCore.DisplayAlertAsync("Alert", "Are you sure?", "Ok").Result;
        }
    }
}
