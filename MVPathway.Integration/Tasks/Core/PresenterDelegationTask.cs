using MVPathway.MVVM.Abstractions;
using System;
using System.Threading.Tasks;
using MVPathway.Integration.Tasks.Base;
using MVPathway.Utils.ViewModels.Qualities;
using MVPathway.Presenters.Abstractions;
using MVPathway.Integration.Tasks.ViewModels;
using MVPathway.Integration.Tasks.Pages;
using MVPathway.Integration.Tasks.ViewModels.Qualities;

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
            _presenter.Init();
            const string cPageException = "Cannot create page";

            try
            {
                _viewModelManager.RegisterPageForViewModel<FirstViewModel, FirstPage>()
                    .AddQuality<IParentQuality>();
                _viewModelManager.RegisterPageForViewModel<SecondViewModel, SecondPage>()
                    .AddQuality<IMainChildQuality>()
                    .AddQuality<MyQuality>();
            }
            catch (Exception e) when (e.Message.StartsWith(cPageException)) { }

            var first = await _presenter.Show<FirstViewModel>();
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

            // close 2nd, returning prev (1st) vm
            first = await _presenter.Close() as FirstViewModel;
            if (second == null || !second.NavFrom)
            {
                return false;
            }
            // try closing 1st, should return null as 1st is root
            first = await _presenter.Close() as FirstViewModel;

            return first == null;

            // TODO : find way to test alerts
            //return PathwayCore.DisplayAlertAsync("Alert", "Are you sure?", "Ok").Result;
        }
    }
}
