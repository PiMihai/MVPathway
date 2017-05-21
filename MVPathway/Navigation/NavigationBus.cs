using MVPathway.Navigation.Abstractions;

namespace MVPathway.Navigation
{
    public class NavigationBus : INavigationBus
    {
        public event OnNavigationRequested CloseRequested;
        public event OnNavigationRequested ShowRequested;
        public event OnAlertRequested AlertRequested;

        public void SendAlert(object sender, NavigationBusAlertEventArgs e)
        {
            AlertRequested?.Invoke(sender, e);
        }

        public void SendClose(object sender, NavigationBusNavigateEventArgs e)
        {
            CloseRequested?.Invoke(sender, e);
        }

        public void SendShow(object sender, NavigationBusNavigateEventArgs e)
        {
            ShowRequested?.Invoke(sender, e);
        }
    }
}
