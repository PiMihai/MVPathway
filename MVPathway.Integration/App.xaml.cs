using MVPathway.Builder.Abstractions;
using MVPathway.Integration.Builder;
using MVPathway.Integration.Services;
using MVPathway.Integration.Services.Contracts;
using MVPathway.MVVM.Abstractions;
using MVPathway.Utils.Builder;

namespace MVPathway.Integration
{
    public partial class App : PathwayApplication
    {
        public App()
        {
            InitializeComponent();
        }

        public override void Configure(IPathwayBuilder builder)
        {
            base.Configure(builder);

            builder.UseAppStart<IntegrationAppStart>()
                   .UseNavigationStackDebugger()
                   .UseLogViewer();
        }

        public override void ConfigureServices(IDiContainer container)
        {
            base.ConfigureServices(container);

            container.Register<IViewModelDefiner, ViewModelDefiner>();
            container.Register<ICacheService, CacheService>();
            //container.Register<ITaskRunner, TaskRunner>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            // TODO : make it work again at some point
            //await Container.Resolve<ITaskRunner>().RunAllTasks();
        }
    }
}
