using MVPathway.Builder.Abstractions;
using MVPathway.MVVM.Abstractions;
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
            app.Container.RegisterInstance(app.Container);

            var builder = new PathwayBuilder(app.Container);

            app.BeforeConfigure();
            app.Configure(builder);
            app.AfterConfigure();
            platformSetup?.Invoke(app.Container);

            builder.Build();

            builder.Container.Resolve<IAppStart>().Start();

            return app;
        }
    }
}
