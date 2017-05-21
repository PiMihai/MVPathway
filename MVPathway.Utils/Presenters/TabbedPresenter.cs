using MVPathway.Presenters;
using MVPathway.MVVM.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;
using MVPathway.Utils.ViewModels.Qualities;
using System.Linq;
using MVPathway.Navigation.Abstractions;

namespace MVPathway.Utils.Presenters
{
    public class TabbedPresenter<TTabbedPage> : BasePresenter
        where TTabbedPage : TabbedPage
    {
        private readonly IViewModelManager _vmManager;
        private readonly INavigationBus _navigationBus;

        private bool _isHandledPop;
        private bool _isHandledTabChange;
        private NavigationPage _navigationPage;
        private TTabbedPage _tabbedPage;

        public TabbedPresenter(IViewModelManager vmManager, INavigationBus navigationBus)
            : base(navigationBus)
        {
            _vmManager = vmManager;
        }

        public override async Task Init()
        {
            _navigationPage = new NavigationPage();
            _tabbedPage = Activator.CreateInstance<TTabbedPage>();
            _tabbedPage.CurrentPageChanged += async (s, e) => await onTabChanged(s, e);
            var childPages = _vmManager.ResolvePagesForViewModels(def => def.HasQuality<IChildQuality>());
            foreach (var child in childPages)
            {
                _tabbedPage.Children.Add(child);
            }
            await _navigationPage.PushAsync(_tabbedPage);
            NavigationPage.SetHasNavigationBar(_tabbedPage, false);
            await OnUiThread(() => Application.Current.MainPage = _navigationPage);
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (viewModel.Definition.HasQuality<IChildQuality>())
            {
                if (!_navigationPage.Navigation.NavigationStack.Contains(_tabbedPage))
                {
                    await _navigationPage.PushAsync(_tabbedPage);
                }
                else
                {
                    while (_navigationPage.CurrentPage != _tabbedPage)
                    {
                        await _navigationPage.PopAsync();
                    }
                }
                _isHandledTabChange = true;
                _tabbedPage.SelectedItem = page;
                _isHandledTabChange = false;
            }
            else
            {
                if (requestType != NavigationRequestType.FromShow)
                {
                    return;
                }
                await _navigationPage.PushAsync(page);
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

        public override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
        {
            return await _navigationPage.DisplayAlert(title, message, okText, cancelText);
        }

        private async Task onTabChanged(object sender, EventArgs e)
        {
            if (_isHandledTabChange)
            {
                return;
            }
            _navigationBus.SendClose(this, new Navigation.NavigationBusNavigateEventArgs
            {
                RequestType = NavigationRequestType.FromClose
            });
        }
    }
}
