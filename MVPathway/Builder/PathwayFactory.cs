using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using System;

namespace MVPathway.Builder
{
  public static class PathwayFactory
  {
    public static TApp Create<TApp>(
      Action<IDiContainer> platformSetup = null)
      where TApp : PathwayApplication
    {
      var app = Activator.CreateInstance<TApp>();
      var builder = new PathwayBuilder();
      app.Configure(builder);
      platformSetup?.Invoke(builder.Container);
      app.Init(builder.Container,
                        builder.Container.Resolve<IViewModelManager>(),
                        builder.Container.Resolve<IMessagingManager>(),
                        builder.Container.Resolve<IPresenter>(),
                        builder.Container.Resolve<ILogger>());

      return app;
    }
  }
}
