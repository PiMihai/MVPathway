using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using MVPathway.Navigation;

namespace MVPathway.Presenters
{
    public abstract class BasePresenter : IPresenter
    {
        protected INavigationBus NavigationBus { get; private set; }

        public BasePresenter(INavigationBus navigationBus)
        {
            NavigationBus = navigationBus;
        }

        public abstract Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null);

        public virtual async Task Init()
        {
            NavigationBus.ShowRequested += onShowRequested;
            NavigationBus.CloseRequested += onCloseRequested;
        }

        public abstract Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType);

        public abstract Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType);

        public virtual async Task Destroy()
        {
            NavigationBus.ShowRequested -= onShowRequested;
            NavigationBus.CloseRequested -= onCloseRequested;
        }

        private void onShowRequested(object sender, NavigationBusNavigateEventArgs e)
        {
            if (!(sender is INavigator))
            {
                return;
            }
            OnShow(e.ViewModel, e.Page, e.RequestType);
        }

        private void onCloseRequested(object sender, NavigationBusNavigateEventArgs e)
        {
            if (!(sender is INavigator))
            {
                return;
            }
            OnClose(e.ViewModel, e.Page, e.RequestType);
        }
    }
}
