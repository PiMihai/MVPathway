namespace MVPathway.Navigation.Abstractions
{
    public interface INavigationBus
    {
        event OnNavigationRequested CloseRequested;
        event OnNavigationRequested ShowRequested;
        event OnAlertRequested AlertRequested;

        void SendShow(object sender, NavigationBusNavigateEventArgs e);
        void SendClose(object sender, NavigationBusNavigateEventArgs e);
        void SendAlert(object sender, NavigationBusAlertEventArgs e);
    }
}
