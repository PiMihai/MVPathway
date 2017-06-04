using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenters.Abstractions
{
    public interface IPresenter
    {
        Task Init();

        Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType);

        Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType);

        Task<bool> OnDisplayAlert(string title, string message, string okText, string cancelText = null);
    }
}
