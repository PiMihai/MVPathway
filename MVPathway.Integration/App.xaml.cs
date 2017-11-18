using MVPathway.Builder.Abstractions;
using MVPathway.Integration.Builder;
using MVPathway.Integration.Pages;
using MVPathway.Integration.Services;
using MVPathway.Integration.Services.Contracts;
using MVPathway.Integration.ViewModels;
using MVPathway.Roam;
using MVPathway.Utils.Builder;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;

namespace MVPathway.Integration
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
            MainPage = new ContentPage();
        }

        public override void BeforeConfigure()
        {
            Container.Register<IViewModelDefiner, ViewModelDefiner>();
            Container.Register<ICacheService, CacheService>();
        }

        public override void Configure(IPathwayBuilder builder)
        {
            base.Configure(builder);

            builder.UseAppStart<IntegrationAppStart>()
                   .UseNavigationStackDebugger()
                   .UseLogViewer()
                   .UseRoam(roamConfig);

            //vmManager.AutoScanAndRegister(GetType().GetAssembly());
        }

        private void roamConfig(IRoamBuilder b)
        {
            b.From<AViewModel>().With<APage>()
               .To<BViewModel>().With<BPage>()
               .Do(setNextAsMain);

            b.From<BViewModel>().With<BPage>()
               .To<AViewModel>().With<APage>()
               .Do(setNextAsMain);

            b.From<AnyViewModel>().With<AnyPage>()
               .To<AViewModel>().With<APage>()
               .Do(setNextAsMain);
        }

        private async Task setNextAsMain(Page p, Page n)
            => await OnUiThread(() => MainPage = n);

        protected override void OnStart()
        {
            base.OnStart();
            // TODO : make it work again at some point
            //await Container.Resolve<ITaskRunner>().RunAllTasks();
        }
    }
}
