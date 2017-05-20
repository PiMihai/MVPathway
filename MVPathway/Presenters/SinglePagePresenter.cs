using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;

namespace MVPathway.Presenters
{
    public class SinglePagePresenter : BasePresenter
    {
        public SinglePagePresenter(IDiContainer container,
                                   IViewModelManager viewModelManager,
                                   IMessagingManager messenger,
                                   ILogger logger)
          : base(container, viewModelManager, messenger, logger)
        {
        }

        public override async Task Init()
        {
            await base.Init();
            Application.Current.MainPage = new ContentPage();
        }

        protected override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            await OnUiThread(() => Application.Current.MainPage = page);
        }

        protected override async Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
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
