using MVPathway.Builder.Abstractions;
using MVPathway.Logging;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using MVPathway.Presenters.Abstractions;
using System;
using Xamarin.Forms;

namespace MVPathway.Builder
{
  public static class PathwayCore
  {
    public static TApp Create<TApp>(IDiContainer diContainer = null,
                                  Action<IPathwayCoreOptions> config = null)
      where TApp : Application
    {
      var container = diContainer ?? new DiContainer();
      container.Register(container);
      container.Register<TApp>();

      var options = new PathwayCoreOptions(container);
      config?.Invoke(options);

      if (!container.IsRegistered<IViewModelManager>())
      {
        container.Register<IViewModelManager, ViewModelManager>();
      }
      if (!container.IsRegistered<IPresenter>())
      {
        container.Register<IPresenter, SinglePagePresenter>();
      }
      if (!container.IsRegistered<IMessagingManager>())
      {
        container.Register<IMessagingManager, MessagingManager>();
      }
      if (!container.IsRegistered<ILogger>())
      {
        container.Register<ILogger, PathwayLogger>();
      }

      return container.Resolve<TApp>();
    }
  }
}
