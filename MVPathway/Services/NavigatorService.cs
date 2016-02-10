using MVPathway.Helpers;
using MVPathway.Services.Contracts;
using Xamarin.Forms;

namespace MVPathway.MVVM
{
    class NavigatorService : INavigatorService
    {
        public void Show<TViewModel>() =>
            MessagingCenter.Send(this, Const.CShowViewModel, typeof(TViewModel));

        public void Close<TViewModel>() =>
            MessagingCenter.Send(this, Const.CCloseViewModel, typeof(TViewModel));
    }
}
