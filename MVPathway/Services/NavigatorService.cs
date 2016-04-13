using MVPathway.Helpers;
using MVPathway.Services.Contracts;
using Xamarin.Forms;

namespace MVPathway.MVVM
{
    class NavigatorService : INavigatorService
    {
        public void Show<TViewModel>(object parameter = null)
        {
            var message = new ViewModelNavigationMessage
            {
                ViewModelType = typeof(TViewModel),
                Parameter = parameter
            };
            MessagingCenter.Send<INavigatorService, ViewModelNavigationMessage>(this, Const.CShowViewModel, message);
        }

        public void Close<TViewModel>(object parameter = null)
        {
            var message = new ViewModelNavigationMessage
            {
                ViewModelType = typeof(TViewModel),
                Parameter = parameter
            };
            MessagingCenter.Send<INavigatorService, ViewModelNavigationMessage>(this, Const.CCloseViewModel, message);
        }
    }
}
