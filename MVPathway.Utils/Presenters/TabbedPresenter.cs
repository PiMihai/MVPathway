using MVPathway.Presenters;
using MVPathway.MVVM.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;
using MVPathway.Utils.ViewModels.Qualities;
using System.Linq;
using MVPathway.Navigation;
using MVPathway.Navigation.Abstractions;

namespace MVPathway.Utils.Presenters
{
    public class TabbedPresenter<TTabbedPage> : BasePresenter
        where TTabbedPage : TabbedPage
    {
        private readonly IViewModelManager _vmManager;

        private bool _isHandledPop;
        private bool _isHandledTabChange;

        private NavigationPage _navigationPage;
        private TTabbedPage _tabbedPage;

        public TabbedPresenter(IViewModelManager vmManager,
                               INavigator navigator)
            : base(navigator)
        {
            _vmManager = vmManager;
        }

        public override async Task Init()
        {
            await base.Init();
            _isHandledTabChange = true;
            _navigationPage = new NavigationPage();
            _navigationPage.Popped += async (s, e) => await onPagePopped(s, e);
            _tabbedPage = Activator.CreateInstance<TTabbedPage>();
            _tabbedPage.CurrentPageChanged += async (s, e) => await onTabChanged(s, e);
            var childPages = _vmManager.ResolvePagesForViewModels(def => def.HasQuality<IChildQuality>());
            foreach (var child in childPages)
            {
                _tabbedPage.Children.Add(child);
                NavigationPage.SetHasNavigationBar(child, true);
            }
            //await _navigationPage.PushAsync(_tabbedPage);
            NavigationPage.SetHasNavigationBar(_tabbedPage, false);
            await OnUiThread(() => Application.Current.MainPage = _navigationPage);
            _isHandledTabChange = false;
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (viewModel.Definition.HasQuality<IChildQuality>())
            {
                await pushOrPopToPage(_tabbedPage);
                _isHandledTabChange = true;
                _tabbedPage.CurrentPage = page;
                _isHandledTabChange = false;
            }
            else
            {
                await pushOrPopToPage(page);
            }
        }

        public override async Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (requestType != NavigationRequestType.FromClose)
            {
                return;
            }

            if (viewModel.Definition.HasQuality<IChildQuality>())
            {
                return;
            }

            _isHandledPop = true;
            await _navigationPage.PopAsync();
            _isHandledPop = false;
        }

        public override async Task<bool> OnDisplayAlert(string title, string message, string okText, string cancelText = null)
        {
            return await _navigationPage.DisplayAlert(title, message, okText, cancelText);
        }

        private async Task pushOrPopToPage(Page page)
        {
            if (!_navigationPage.Navigation.NavigationStack.Contains(page))
            {
                await _navigationPage.PushAsync(page);
            }
            else
            {
                while (_navigationPage.CurrentPage != page)
                {
                    await _navigationPage.PopAsync();
                }
            }
        }

        private async Task onTabChanged(object sender, EventArgs e)
        {
            if (_isHandledTabChange)
            {
                return;
            }
            var newVm = _tabbedPage.CurrentPage.BindingContext as BaseViewModel;
            if (newVm == null)
            {
                return;
            }
            await Navigator.Show(newVm);
        }

        private async Task onPagePopped(object sender, EventArgs e)
        {
            if (_isHandledPop)
            {
                return;
            }
            await Navigator.Close(this);
        }
    }
}
