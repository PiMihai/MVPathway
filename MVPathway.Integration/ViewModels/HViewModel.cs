using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public class HViewModel : _ViewModel
    {
        public override string Title => "H";
        public override Color Color => HColor;

        public HViewModel(INavigator navigator, NavigationStackDebuggerViewObject stackDebugger)
            : base(navigator, stackDebugger)
        {
        }
    }
}
