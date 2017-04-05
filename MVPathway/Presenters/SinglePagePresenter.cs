using MVPathway.Logging.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenters
{
    public class SinglePagePresenter : BasePresenter
    {
        public SinglePagePresenter(ILogger logger, IViewModelManager viewModelManager, IDiContainer container)
          : base(container, viewModelManager, logger)
        {
        }

        public override async Task<BaseViewModel> Show(BaseViewModel viewModel, object parameter)
        {
            await base.Show(viewModel, parameter);
            var page = ViewModelManager.ResolvePageForViewModel(viewModel);
            if(page == null)
            {
                return null;
            }
            Application.Current.MainPage = page;
            return viewModel;
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
