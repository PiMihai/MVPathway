using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenters
{
    public abstract class BasePresenter : IPresenter
    {
        protected INavigationBus NavigationBus { get; private set; }

        public BasePresenter(INavigationBus navigationBus)
        {
            NavigationBus = navigationBus;
            navigationBus.ShowRequested += (s, e) =>
            {
                if (!(s is INavigator))
                {
                    return;
                }
                OnShow(e.ViewModel, e.Page, e.RequestType);
            };
            navigationBus.CloseRequested += (s, e) =>
            {
                if (!(s is INavigator))
                {
                    return;
                }
                OnClose(e.ViewModel, e.Page, e.RequestType);
            };
        }

        public abstract Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null);

        public abstract Task Init();

        public abstract Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType);

        public abstract Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType);
    }
}
