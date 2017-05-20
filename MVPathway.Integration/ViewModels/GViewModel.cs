using MVPathway.Presenters.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public class GViewModel : _ViewModel
    {
        public override string Title => "G";
        public override Color Color => GColor;

        public GViewModel(IPresenter presenter, NavigationStackDebuggerViewObject stackDebugger)
            : base(presenter, stackDebugger)
        {
        }
    }
}
