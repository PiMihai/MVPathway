using MVPathway.Builder.Abstractions;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;

namespace MVPathway.Builder
{
  class PathwayCoreOptions : IPathwayCoreOptions
  {
    private readonly IDiContainer mContainer;

    public PathwayCoreOptions(IDiContainer container)
    {
      mContainer = container;
    }

    public void UseLogger<TLogger>() where TLogger : ILogger
    {
      mContainer.Register<ILogger, TLogger>();
    }

    public void UseMessagingManager<TMessagingManager>() where TMessagingManager : IMessagingManager
    {
      mContainer.Register<IMessagingManager, TMessagingManager>();
    }

    public void UsePresenter<TPresenter>() where TPresenter : IPresenter
    {
      mContainer.Register<IPresenter, TPresenter>();
    }

    public void UseViewModelManager<TViewModelManager>() where TViewModelManager : IViewModelManager
    {
      mContainer.Register<IViewModelManager, TViewModelManager>();
    }
  }
}
