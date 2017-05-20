using MVPathway.Presenters.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public class FViewModel : _ViewModel
    {
        public override string Title => "F";
        public override Color Color => FColor;

        public FViewModel(IPresenter presenter, NavigationStackDebuggerViewObject stackDebugger)
            : base(presenter, stackDebugger)
        {
        }
    }
}
