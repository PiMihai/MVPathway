using MVPathway.Logging;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;

namespace MVPathway
{
  public static class PathwayCoreFactory
  {
    public static IPathwayCore Create()
    {
      var container = new DiContainer();
      container.Register<IDiContainer>(container);
      container.Register<IViewModelManager, ViewModelManager>();
      container.Register<IMessagingManager, MessagingManager>();
      container.Register<ILogger, PathwayLogger>();
      container.Register<IPathwayCore, PathwayCore>();

      return container.Resolve<IPathwayCore>();
    }
  }
}
