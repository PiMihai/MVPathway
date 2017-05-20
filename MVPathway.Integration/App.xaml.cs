using MVPathway.MVVM.Abstractions;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.Presenters.Abstractions;
using Xamarin.Forms;
using MVPathway.Integration.Services.Contracts;
using MVPathway.Integration.Services;
using MVPathway.Integration.ViewModels;
using MVPathway.Integration.Pages;
using MVPathway.Integration.Converters;
using MVPathway.Presenters;
using MVPathway.Utils.Presenters;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using MVPathway.Utils.ViewModels.Qualities;
using MVPathway.Builder.Abstractions;
using MVPathway.Utils.Builder;
using System.Threading.Tasks;

namespace MVPathway.Integration
{
    public partial class App
    {
        private IViewModelManager _vmManager;
        private Dictionary<Type, ViewModelDefinition> _vmDefs = new Dictionary<Type, ViewModelDefinition>();

        public IDiContainer Container { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        public override void Configure(IPathwayBuilder builder)
        {
            base.Configure(builder);
            builder.UsePresenter<MasterDetailPresenter<MasterDetailPage>>();
            builder.UseNavigationStackDebugger();
        }

        public override async void Init(IDiContainer container,
                                        IViewModelManager vmManager,
                                        IMessagingManager messagingManager,
                                        IPresenter presenter,
                                        ILogger logger)
        {
            Container = container;
            _vmManager = vmManager;

            container.Register<ITaskRunner, TaskRunner>();

            var presenterConverter = Current.Resources["PresenterTypeListToStringConverter"] as PresenterTypeListToStringConverter;
            Current.Resources["PresenterTypeToStringConverter"] = presenterConverter.ForItem;
            presenterConverter.SupportedPresenters = new ObservableCollection<Type>{
                typeof(SinglePagePresenter),
                typeof(StackPresenter<NavigationPage>),
                typeof(MasterDetailPresenter<MasterDetailPage>) };

            _vmDefs[typeof(MainViewModel)] = vmManager.RegisterPageForViewModel<MainViewModel, MainPage>()
                .AddQuality<IParentQuality>();

            await SetupWithPresenter(presenter);

            await presenter.Show<MainViewModel>();
        }

        public async Task SetupWithPresenter(IPresenter presenter)
        {
            await ensureNoPageHasParent();

            _vmDefs[typeof(AViewModel)] = _vmManager.RegisterPageForViewModel<AViewModel, APage>();
            _vmDefs[typeof(BViewModel)] = _vmManager.RegisterPageForViewModel<BViewModel, BPage>();
            _vmDefs[typeof(CViewModel)] = _vmManager.RegisterPageForViewModel<CViewModel, CPage>();
            _vmDefs[typeof(DViewModel)] = _vmManager.RegisterPageForViewModel<DViewModel, DPage>();
            _vmDefs[typeof(EViewModel)] = _vmManager.RegisterPageForViewModel<EViewModel, EPage>();
            _vmDefs[typeof(FViewModel)] = _vmManager.RegisterPageForViewModel<FViewModel, FPage>();
            _vmDefs[typeof(GViewModel)] = _vmManager.RegisterPageForViewModel<GViewModel, GPage>();
            _vmDefs[typeof(HViewModel)] = _vmManager.RegisterPageForViewModel<HViewModel, HPage>();

            if (presenter is MasterDetailPresenter<MasterDetailPage>)
            {
                _vmDefs[typeof(AViewModel)].AddQuality<IMainChildQuality>();

                _vmDefs[typeof(BViewModel)].AddQuality<IChildQuality>();

                _vmDefs[typeof(CViewModel)].AddQuality<IChildQuality>();
                _vmDefs[typeof(CViewModel)].AddQuality<IFullscreenQuality>();

                _vmDefs[typeof(DViewModel)].AddQuality<IChildQuality>();
                _vmDefs[typeof(DViewModel)].AddQuality<IFullscreenQuality>();

                _vmDefs[typeof(EViewModel)].AddQuality<IModalQuality>();

                _vmDefs[typeof(FViewModel)].AddQuality<IModalQuality>();

                _vmDefs[typeof(GViewModel)].AddQuality<IModalQuality>();
                _vmDefs[typeof(GViewModel)].AddQuality<IFullscreenQuality>();

                _vmDefs[typeof(HViewModel)].AddQuality<IModalQuality>();
                _vmDefs[typeof(HViewModel)].AddQuality<IFullscreenQuality>();
            }
        }

        protected override async void OnStart()
        {
            base.OnStart();
            // TODO : make it work again at some point
            //await Container.Resolve<ITaskRunner>().RunAllTasks();
        }

        private async Task ensureNoPageHasParent()
        {
            if (Current.MainPage is NavigationPage navigationPage)
            {
                await navigationPage.PopToRootAsync();
                var oldPage = navigationPage.CurrentPage;
                await navigationPage.PushAsync(new ContentPage());
                navigationPage.Navigation.RemovePage(oldPage);
            }
            Current.MainPage = new ContentPage();
        }
    }
}
