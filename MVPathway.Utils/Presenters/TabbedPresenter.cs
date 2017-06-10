using MVPathway.MVVM.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using MVPathway.Utils.ViewModels.Qualities;
using MVPathway.Navigation;
using MVPathway.Navigation.Abstractions;

namespace MVPathway.Utils.Presenters
{
    public class TabbedPresenter : TabbedPresenter<TabbedPage, NavigationPage>
    {
        public TabbedPresenter(IViewModelManager vmManager, INavigator navigator)
            : base(vmManager, navigator)
        {
        }
    }
    public class TabbedPresenter<TTabbedPage> : TabbedPresenter<TTabbedPage, NavigationPage>
        where TTabbedPage : TabbedPage
    {
        public TabbedPresenter(IViewModelManager vmManager, INavigator navigator)
            : base(vmManager, navigator)
        {
        }
    }

    public class TabbedPresenter<TTabbedPage, TNavigationPage>
        : StackPresenter<TNavigationPage>
        where TTabbedPage : TabbedPage
        where TNavigationPage : NavigationPage
    {
        private readonly IViewModelManager _vmManager;

        protected TTabbedPage TabbedPage { get; set; }

        public TabbedPresenter(IViewModelManager vmManager,
                               INavigator navigator)
            : base(navigator)
        {
            _vmManager = vmManager;
        }

        public override async Task Init()
        {
            await base.Init();
            TabbedPage = Activator.CreateInstance<TTabbedPage>();
            TabbedPage.CurrentPageChanged += async (s, e) => await onTabChanged(s, e);
            var childPages = _vmManager.ResolvePagesForViewModels(def => def.HasQuality<IChildQuality>());
            foreach (var child in childPages)
            {
                TabbedPage.Children.Add(child);
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(child, true);
            }
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (viewModel.Definition.HasQuality<IChildQuality>())
            {
                TabbedPage.CurrentPage = page;
                TabbedPage.Title = page.Title;

                await base.OnShow(viewModel, TabbedPage, requestType);
                return;
            }
            await base.OnShow(viewModel, page, requestType);
        }

        private async Task onTabChanged(object sender, EventArgs e)
        {
            if (Navigator.DuringRequestedTransition)
            {
                return;
            }
            var newVm = TabbedPage.CurrentPage.BindingContext as BaseViewModel;
            if (newVm == null)
            {
                return;
            }
            await Navigator.Show(newVm);
        }
    }
}
