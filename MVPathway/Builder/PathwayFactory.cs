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
            var builder = new PathwayBuilder();

            app.Configure(builder);
            builder.Build();

            app.ConfigureServices(builder.Container);
            platformSetup?.Invoke(builder.Container);

            builder.Container.Resolve<IAppStart>().Start();

            return app;
        }
    }
}
