using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using System;
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

        public StackPresenter(IDiContainer container,
                              IViewModelManager viewModelManager,
                              IMessagingManager messenger,
                              ILogger logger)
          : base(container, viewModelManager, messenger, logger)
        {
        }

        public override async Task Init()
        {
            await base.Init();
            _navigationPage = Activator.CreateInstance<TNavigationPage>();
            _navigationPage.Popped += async (sender, e) => await onNavigationPagePopped(sender, e);
        }

        protected override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            await _navigationPage.PushAsync(page);
            if (Application.Current.MainPage != _navigationPage)
            {
                Application.Current.MainPage = _navigationPage;
            }
        }

        protected override async Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (Application.Current.MainPage != _navigationPage)
            {
                Application.Current.MainPage = _navigationPage;
            }
            _isHandledPop = true;
            await _navigationPage.PopAsync();
            _isHandledPop = false;
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
            var viewModel = e.Page.BindingContext as BaseViewModel;
            if (viewModel == null)
            {
                return;
            }
            await Close();
        }
    }
}
