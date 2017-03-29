using System.Threading.Tasks;
using MVPathway.Integration.Tasks.Core;
using MVPathway.Utils.Presenters;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using Xamarin.Forms;

namespace MVPathway.Integration.Tasks.Utils
{
  public class NavigationPagePresenterDelegationTask : BaseUtilsPresenterDelegationTask
  {
    public NavigationPagePresenterDelegationTask(IDiContainer container, IViewModelManager viewModelManager)
      : base(container, viewModelManager) { }

    public override async Task<bool> Execute()
    {
      Container.Register<IPresenter, NavigationPagePresenter>();
      return await base.Execute();
    }
  }

  public class NavigableMasterDetailPresenterDelegationTask : BaseUtilsPresenterDelegationTask
  {
    public NavigableMasterDetailPresenterDelegationTask(IDiContainer container, IViewModelManager viewModelManager)
      : base(container, viewModelManager) { }

    public override async Task<bool> Execute()
    {
      Container.Register<IPresenter, NavigableMasterDetailPresenter<MasterDetailPage>>();
      return await base.Execute();
    }
  }

  public abstract class BaseUtilsPresenterDelegationTask : PresenterDelegationTask
  {
    protected IDiContainer Container { get; private set; }

    public BaseUtilsPresenterDelegationTask(IDiContainer container,
                                            IViewModelManager viewModelManager)
      : base(container, viewModelManager)
    {
      Container = container;
    }
  }
}
