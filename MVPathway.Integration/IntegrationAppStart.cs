using MVPathway.Builder.Abstractions;
using MVPathway.Integration.Services.Contracts;
using MVPathway.Integration.ViewModels;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters;
using MVPathway.Utils.Presenters;

namespace MVPathway.Integration
{
    public class IntegrationAppStart : IAppStart
    {
        private readonly IViewModelDefiner _vmDefiner;
        private readonly INavigator _navigator;

        public IntegrationAppStart(IViewModelDefiner vmDefiner,
                                   INavigator navigator)
        {
            _vmDefiner = vmDefiner;
            _navigator = navigator;
        }

        public async void Start()
        {
            _vmDefiner.Init();
            await _vmDefiner.RedefineBasedOnPresenterType(typeof(TabbedPresenter));

            await _navigator.Show<AViewModel>();
        }
    }
}
