using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public class BViewModel : _ViewModel
    {
        public override string Title => "B";
        public override Color Color => BColor;

        public BViewModel(INavigator navigator, NavigationStackDebuggerViewObject stackDebugger)
            : base(navigator, stackDebugger)
        {
        }
    }
}
