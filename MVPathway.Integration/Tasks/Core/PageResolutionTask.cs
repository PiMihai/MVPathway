using System.Threading.Tasks;
using MVPathway.Integration.Tasks.Base;
using MVPathway.MVVM.Abstractions;
using MVPathway.Integration.Tasks.ViewModels;
using MVPathway.Integration.Tasks.Pages;
using MVPathway.Integration.Tasks.ViewModels.Qualities;

namespace MVPathway.Integration.Tasks.Core
{
    public class PageResolutionTask : CoreIntegrationTask
    {
        private readonly IDiContainer mContainer;
        private readonly IViewModelManager _viewModelManager;

        public PageResolutionTask(IDiContainer container, IViewModelManager viewModelManager)
        {
            mContainer = container;
            _viewModelManager = viewModelManager;
        }

        public override async Task<bool> Execute()
        {
            await base.Execute();

            _viewModelManager.RegisterPageForViewModel<FirstViewModel, FirstPage>();
            _viewModelManager.RegisterPageForViewModel<SecondViewModel, SecondPage>()
              .AddQuality<MyQuality>();

            var firstPage = _viewModelManager.ResolvePageForViewModel(mContainer.Resolve<FirstViewModel>());
            if (!(firstPage is FirstPage))
            {
                return false;
            }

            var secondPage = _viewModelManager.ResolvePageForViewModel(x => x.HasQuality<MyQuality>());
            return secondPage is SecondPage;
        }
    }
}
