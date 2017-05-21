using MVPathway.MVVM.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;
using MVPathway.Navigation.Abstractions;

namespace MVPathway.Presenters
{
    public class SinglePagePresenter : BasePresenter
    {
        public SinglePagePresenter(INavigationBus navigationBus) : base(navigationBus)
        {
        }

        public override async Task Init()
        {
            await OnUiThread(() => Application.Current.MainPage = new ContentPage());
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            await OnUiThread(() => Application.Current.MainPage = page);
        }

        public override async Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
        }

        public override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
        {
            if (cancelText != null)
            {
                return await Application.Current.MainPage?.DisplayAlert(title, message, okText, cancelText);
            }
            await Application.Current.MainPage?.DisplayAlert(title, message, okText);
            return true;
        }
    }
}
