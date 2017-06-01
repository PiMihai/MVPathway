using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;

namespace MVPathway.Utils.Presenters
{
    public class StackPresenter<TNavigationPage> : BasePresenter
        where TNavigationPage : NavigationPage
    {
        private const string ERROR_INVALID_VM_TO_CLOSE = "TViewModel to close is not the same type as the one on top of the stack.";

        private bool _isHandledPop;
        private TNavigationPage _navigationPage;

        public bool Animated { get; set; }

        public StackPresenter(INavigationBus navigationBus) : base(navigationBus)
        {
        }

        public override async Task Init()
        {
            await base.Init();
            _navigationPage = Activator.CreateInstance<TNavigationPage>();
            _navigationPage.Popped += async (sender, e) => await onNavigationPagePopped(sender, e);
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (requestType == NavigationRequestType.FromClose)
            {
                if (!_navigationPage.Navigation.NavigationStack.Contains(page))
                {
                    await OnUiThread(async () =>
                    {
                        await _navigationPage.PushAsync(page, Animated);
                        Application.Current.MainPage = _navigationPage;
                    });
                }
                return;
            }

            if (_navigationPage.Navigation.NavigationStack.Contains(page))
            {
                var tempStack = new Stack<Page>();
                _isHandledPop = true;
                while (_navigationPage.CurrentPage != page)
                {
                    tempStack.Push(await _navigationPage.PopAsync(false));
                }
                await _navigationPage.PopAsync(false);
                _isHandledPop = false;
                while (tempStack.Count > 0)
                {
                    await _navigationPage.PushAsync(tempStack.Pop(), false);
                }
            }
            await _navigationPage.PushAsync(page, Animated);
            Application.Current.MainPage = _navigationPage;
        }

        public override async Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (requestType != NavigationRequestType.FromClose)
            {
                return;
            }

            await OnUiThread(async () =>
            {
                _isHandledPop = true;
                await _navigationPage.PopAsync(Animated);
                _isHandledPop = false;
                Application.Current.MainPage = _navigationPage;
            });
        }

        public override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
        {
            return await _navigationPage.DisplayAlert(title, message, okText, cancelText);
        }

        private async Task onNavigationPagePopped(object sender, NavigationEventArgs e)
        {
            if (_isHandledPop)
            {
                return;
            }
            NavigationBus.SendClose(this, new NavigationBusNavigateEventArgs
            {
                RequestType = NavigationRequestType.FromClose
            });
        }
    }
}
