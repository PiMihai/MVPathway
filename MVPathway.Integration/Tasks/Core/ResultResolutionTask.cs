using System.Threading.Tasks;
using MVPathway.Integration.Tasks.Base;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using MVPathway.Integration.Tasks.ViewModels;
using MVPathway.Integration.Tasks.Pages;

namespace MVPathway.Integration.Tasks.Core
{
    class ResultResolutionTask : CoreIntegrationTask
    {
        private readonly IViewModelManager mVmManager;
        private readonly IPresenter mPresenter;

        public ResultResolutionTask(IViewModelManager vmManager, IPresenter presenter)
        {
            mVmManager = vmManager;
            mPresenter = presenter;
        }

        public override async Task<bool> Execute()
        {
            await base.Execute();
            mVmManager.RegisterPageForViewModel<OkResultViewModel, FirstPage>();
            var result = await mPresenter.GetResult<OkResultViewModel, string>();
            return result.Result == "Ok";
        }
    }
}
