using MVPathway.Integration.Tasks.Core;
using MVPathway.Utils.Presenters;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;

namespace MVPathway.Integration.Tasks.Utils
{
  public class NavigationPagePresenterDelegationTask : BaseUtilsPresenterDelegationTask
  {
    public NavigationPagePresenterDelegationTask(IDiContainer container, IPresenter presenter, IViewModelManager viewModelManager) : base(container, presenter, viewModelManager)
    {
    }

    public override bool Execute()
    {
      Container.Register<IPresenter, NavigationPagePresenter>();
      return base.Execute();
    }
  }

  public class NavigableMasterDetailPresenterDelegationTask : BaseUtilsPresenterDelegationTask
  {
    public NavigableMasterDetailPresenterDelegationTask(IDiContainer container, IPresenter presenter, IViewModelManager viewModelManager) : base(container, presenter, viewModelManager)
    {
    }

    public override bool Execute()
    {
      Container.Register<IPresenter, NavigableMasterDetailPresenter>();
      return base.Execute();
    }
  }

  public abstract class BaseUtilsPresenterDelegationTask : PresenterDelegationTask
  {
    protected IDiContainer Container { get; private set; }

    public BaseUtilsPresenterDelegationTask(IDiContainer container,
                                            IPresenter presenter,
                                            IViewModelManager viewModelManager)
      : base(presenter, viewModelManager)
    {
      Container = container;
    }
  }
}
