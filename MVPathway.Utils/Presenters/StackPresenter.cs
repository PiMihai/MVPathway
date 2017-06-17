using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters;
using MVPathway.Utils.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;

namespace MVPathway.Utils.Presenters
{
    public class StackPresenter : StackPresenter<NavigationPage>
    {
        public StackPresenter(INavigator navigator)
            : base(navigator)
        {
        }
    }

    public class StackPresenter<TNavigationPage> : BasePresenter
        where TNavigationPage : NavigationPage
    {
        protected TNavigationPage NavigationPage { get; set; }

        public StackPresenter(INavigator navigator)
            : base(navigator)
        {
        }

        public override async Task Init()
        {
            await base.Init();
            NavigationPage = Activator.CreateInstance<TNavigationPage>();
            NavigationPage.Popped += async (sender, e) => await onNavigationPagePopped(sender, e);
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            await NavigationPage.PushOrPopToPage(page, Animated);
            await OnUiThread(() => Application.Current.MainPage = NavigationPage);
        }

        public override async Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
        }

        public override async Task<bool> OnDisplayAlert(string title, string message, string okText, string cancelText = null)
        {
            if (cancelText != null)
            {
                return await NavigationPage?.DisplayAlert(title, message, okText, cancelText);
            }
            await NavigationPage?.DisplayAlert(title, message, okText);
            return true;
        }

        private async Task onNavigationPagePopped(object sender, NavigationEventArgs e)
        {
            if (Navigator.DuringRequestedTransition)
            {
                return;
            }
            await Navigator.Close(this);
        }
    }
}
