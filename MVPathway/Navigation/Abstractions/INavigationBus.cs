using MVPathway.Presenters.Abstractions;
using System;
using System.Threading.Tasks;

namespace MVPathway.Navigation.Abstractions
{
    public interface INavigationBus
    {
        event OnNavigationRequested CloseRequested;
        event OnNavigationRequested ShowRequested;
        event OnAlertRequested AlertRequested;
        event Action NavigationStackCleared;

        void SendShow(object sender, NavigationBusNavigateEventArgs e);
        void SendClose(object sender, NavigationBusNavigateEventArgs e);
        void SendAlert(object sender, NavigationBusAlertEventArgs e);
        void ClearNavigationStack();

        Task ChangePresenterTo<TPresenter>()
            where TPresenter : IPresenter;

        Task ChangePresenterTo(Type presenterType);
    }
}
