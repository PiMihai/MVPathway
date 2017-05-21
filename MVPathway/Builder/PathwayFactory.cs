using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
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
            var presenter = builder.Container.Resolve<IPresenter>();
            builder.ConfigurePresenter?.Invoke(presenter);
            presenter.Init();
            app.Init(builder.Container,
                              builder.Container.Resolve<IViewModelManager>(),
                              builder.Container.Resolve<IMessenger>(),
                              builder.Container.Resolve<INavigator>(),
                              builder.Container.Resolve<ILogger>());

            return app;
        }
    }
}
