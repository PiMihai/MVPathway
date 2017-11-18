using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenters
{
    public abstract class BasePresenter : IPresenter
    {
        protected INavigator Navigator { get; private set; }

        public bool Animated { get; set; } = true;

        public BasePresenter(INavigator navigator)
        {
            Navigator = navigator;
        }

        public abstract Task<bool> OnDisplayAlert(string title, string message, string okText, string cancelText = null);

        public virtual Task Init() => Task.CompletedTask;

        public abstract Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType);

        public abstract Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType);
    }
}
