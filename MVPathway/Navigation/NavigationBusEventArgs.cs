using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using System;
using Xamarin.Forms;

namespace MVPathway.Navigation
{
    public delegate void OnNavigationRequested(object sender, NavigationBusNavigateEventArgs e);
    public delegate void OnAlertRequested(object sender, NavigationBusAlertEventArgs e);

    public class NavigationBusNavigateEventArgs : EventArgs
    {
        public BaseViewModel ViewModel { get; set; }
        public Page Page { get; set; }
        public NavigationRequestType RequestType { get; set; }
    }

    public class NavigationBusAlertEventArgs : EventArgs
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string OkText { get; set; }
        public string CancelText { get; set; }
    }
}
