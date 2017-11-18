using MVPathway.MVVM.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;
using MVPathway.Navigation.Abstractions;
using MVPathway.Navigation;

namespace MVPathway.Presenters
{
    public class SinglePagePresenter : BasePresenter
    {
        public SinglePagePresenter(INavigator navigator)
            : base(navigator)
        {
        }

        public override async Task Init()
        {
            await base.Init();
            await OnUiThread(() => Application.Current.MainPage = new ContentPage());
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            await OnUiThread(() => Application.Current.MainPage = page);
        }

        public override Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
            => Task.CompletedTask;

        public override async Task<bool> OnDisplayAlert(string title, string message, string okText, string cancelText = null)
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
