using MVPathway.Builder.Abstractions;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using System;

namespace MVPathway.Builder
{
  class PathwayBuilder : IPathwayBuilder
  {
    public IDiContainer Container { get; private set; }
    
    public void SetupPlatform(Action<IDiContainer> platformSetup)
    {
      ensureDiContainerLoaded();
      platformSetup(Container);
    }

    public IPathwayBuilder UseDiContainer<TDiContainer>()
      where TDiContainer : IDiContainer
    {
      Container = Activator.CreateInstance<TDiContainer>();
      Container.Register(Container);

      return this;
    }

    public IPathwayBuilder UseLogger<TLogger>() where TLogger : ILogger
    {
      ensureDiContainerLoaded();
      Container.Register<ILogger, TLogger>();

      return this;
    }

    public IPathwayBuilder UseMessagingManager<TMessagingManager>() where TMessagingManager : IMessagingManager
    {
      ensureDiContainerLoaded();
      Container.Register<IMessagingManager, TMessagingManager>();

      return this;
    }

    public IPathwayBuilder UsePresenter<TPresenter>() where TPresenter : IPresenter
    {
      ensureDiContainerLoaded();
      Container.Register<IPresenter, TPresenter>();

      return this;
    }

    public IPathwayBuilder UseViewModelManager<TViewModelManager>() where TViewModelManager : IViewModelManager
    {
      ensureDiContainerLoaded();
      Container.Register<IViewModelManager, TViewModelManager>();

      return this;
    }

    private void ensureDiContainerLoaded()
    {
      if(Container == null)
      {
        Container = new DiContainer();
      }
    }
  }
}
