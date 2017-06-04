using MVPathway.Integration.Services.Contracts;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public class GViewModel : _ViewModel
    {
        public override Color Color => GColor;

        public GViewModel(INavigator navigator,
                          IDiContainer container,
                          IViewModelDefiner vmDefiner,
                          ICacheService cacheService,
                          NavigationStackDebuggerViewObject stackDebugger)
            : base(navigator, container, vmDefiner, cacheService, stackDebugger)
        {
        }
    }
}
