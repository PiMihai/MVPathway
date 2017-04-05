using MVPathway.Logging.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Utils.Presenters
{
    public class NavigationPagePresenter : BasePresenter
    {
        private const string cInvalidViewModelToCloseError = "TViewModel to close is not the same type as the one on top of the stack.";

        private readonly NavigationPage mNavigationPage;

        public NavigationPagePresenter(IViewModelManager viewModelManager,
                                       IDiContainer container,
                                       ILogger logger)
          : base(container, viewModelManager, logger)
        {
            Application.Current.MainPage = mNavigationPage = new NavigationPage();
        }

        public override async Task<BaseViewModel> Show(BaseViewModel viewModel, object parameter)
        {
            if(await base.Show(viewModel, parameter) == null)
            {
                return null;
            }

            var page = ViewModelManager.ResolvePageForViewModel(viewModel);
            await mNavigationPage.PushAsync(page);
            return viewModel;
        }

        public override async Task<BaseViewModel> Close(BaseViewModel viewModel, object parameter)
        {
            if(await base.Close(viewModel, parameter) == null)
            {
                return null;
            }
            await mNavigationPage.PopAsync();
            return viewModel;
        }

        public override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
        {
            return await mNavigationPage.DisplayAlert(title, message, okText, cancelText);
        }
    }
}
