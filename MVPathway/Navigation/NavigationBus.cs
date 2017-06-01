using System;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using MVPathway.MVVM.Abstractions;
using System.Threading.Tasks;

namespace MVPathway.Navigation
{
    public class NavigationBus : INavigationBus
    {
        private readonly IDiContainer _container;

        public event OnNavigationRequested CloseRequested;
        public event OnNavigationRequested ShowRequested;
        public event OnAlertRequested AlertRequested;
        public event Action NavigationStackCleared;

        public NavigationBus(IDiContainer container)
        {
            _container = container;
        }

        public async Task ChangePresenterTo<TPresenter>() where TPresenter : IPresenter
        {
            await ChangePresenterTo(typeof(TPresenter));
        }

        public async Task ChangePresenterTo(Type presenterType)
        {
            if (_container.IsRegistered<IPresenter>())
            {
                await _container.Resolve<IPresenter>().Destroy();
            }
            ClearNavigationStack();
            _container.Register(presenterType);
            var presenterInstance = _container.Resolve(presenterType) as IPresenter;
            _container.RegisterInstance(presenterInstance);
            await presenterInstance.Init();
        }

        public void ClearNavigationStack()
        {
            NavigationStackCleared?.Invoke();
        }

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
