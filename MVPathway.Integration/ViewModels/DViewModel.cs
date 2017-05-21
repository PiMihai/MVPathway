using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public class DViewModel : _ViewModel
    {
        public override string Title => "D";
        public override Color Color => DColor;

        public DViewModel(INavigator navigator, NavigationStackDebuggerViewObject stackDebugger)
            : base(navigator, stackDebugger)
        {
        }
    }
}
