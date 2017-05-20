using MVPathway.Presenters.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public class AViewModel : _ViewModel
    {
        public override string Title => "A";
        public override Color Color => AColor;

        public AViewModel(IPresenter presenter, NavigationStackDebuggerViewObject stackDebugger)
            : base(presenter, stackDebugger)
        {
        }
    }
}
