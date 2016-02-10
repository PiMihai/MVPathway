using MVPathway.Helpers;
using MVPathway.Services.Contracts;
using System;
using Xamarin.Forms;

namespace MVPathway.MVVM
{
    class NavigatorService : INavigatorService
    {
        public void Show<TViewModel>() =>
            MessagingCenter.Send<INavigatorService,Type>(this, Const.CShowViewModel, typeof(TViewModel));

        public void Close<TViewModel>() =>
            MessagingCenter.Send<INavigatorService, Type>(this, Const.CCloseViewModel, typeof(TViewModel));
    }
}
