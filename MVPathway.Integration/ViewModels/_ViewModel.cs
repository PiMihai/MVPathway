using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public abstract class _ViewModel : BaseViewModel
    {
        private readonly INavigator _navigator;

        public abstract string Title { get; }
        public abstract Color Color { get; }

        public Color AColor => Color.Aqua;
        public Color BColor => Color.Beige;
        public Color CColor => Color.Crimson;
        public Color DColor => Color.DeepSkyBlue;
        public Color EColor => Color.Gray;
        public Color FColor => Color.Fuchsia;
        public Color GColor => Color.Green;
        public Color HColor => Color.Honeydew;

        private Command _aCommand;
        public Command ACommand => _aCommand ?? (_aCommand = new Command(
            async () => await _navigator.Show<AViewModel>()));

        private Command _bCommand;
        public Command BCommand => _bCommand ?? (_bCommand = new Command(
            async () => await _navigator.Show<BViewModel>()));

        private Command _cCommand;
        public Command CCommand => _cCommand ?? (_cCommand = new Command(
            async () => await _navigator.Show<CViewModel>()));

        private Command _dCommand;
        public Command DCommand => _dCommand ?? (_dCommand = new Command(
            async () => await _navigator.Show<DViewModel>()));

        private Command _eCommand;
        public Command ECommand => _eCommand ?? (_eCommand = new Command(
            async () => await _navigator.Show<EViewModel>()));

        private Command _fCommand;
        public Command FCommand => _fCommand ?? (_fCommand = new Command(
            async () => await _navigator.Show<FViewModel>()));

        private Command _gCommand;
        public Command GCommand => _gCommand ?? (_gCommand = new Command(
            async () => await _navigator.Show<GViewModel>()));

        private Command _hCommand;
        public Command HCommand => _hCommand ?? (_hCommand = new Command(
            async () => await _navigator.Show<HViewModel>()));

        private Command _closeCommand;

        public Command CloseCommand => _closeCommand ?? (_closeCommand = new Command(
            async () => await _navigator.Close()));

        public NavigationStackDebuggerViewObject StackDebugger { get; private set; }

        public _ViewModel(INavigator navigator, NavigationStackDebuggerViewObject stackDebugger)
        {
            _navigator = navigator;
            StackDebugger = stackDebugger;
        }
    }
}
