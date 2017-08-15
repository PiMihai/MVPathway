using MVPathway.Integration.Services.Contracts;
using MVPathway.Integration.ViewModels.ViewObjects;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters;
using MVPathway.Utils.Presenters;
using MVPathway.Utils.ViewModels.ViewObjects;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Integration.ViewModels
{
    public abstract class _ViewModel : BaseViewModel
    {
        private readonly INavigator _navigator;
        private readonly IViewModelDefiner _vmDefiner;
        private readonly ICacheService _cacheService;

        public string Title { get; }
        public abstract Color Color { get; }

        private ObservableCollection<Type> _presenters;
        public ObservableCollection<Type> Presenters
        {
            get => _presenters;
            set
            {
                _presenters = value;
                OnPropertyChanged();
            }
        }

        public Type SelectedPresenter
        {
            get => _cacheService.PresenterType;
            set
            {
                if (value == null || _cacheService.PresenterType == value)
                {
                    return;
                }
                _cacheService.PresenterType = value;
                OnPropertyChanged();
                ChangePresenterCommand.Execute(value);
            }
        }

        private Command<Type> _changePresenterCommand;
        public Command<Type> ChangePresenterCommand => _changePresenterCommand ?? (_changePresenterCommand = new Command<Type>(
            async (p) => await changePresenter(p)));

        private async Task changePresenter(Type presenterType)
        {
            await _vmDefiner.RedefineBasedOnPresenterType(presenterType);
            await _navigator.ChangePresenter(presenterType, p =>
            {
                var bp = (BasePresenter)p;
                bp.Animated = false;
            });
        }

        public Color AColor => Color.FromHex("#F44336");
        public Color BColor => Color.FromHex("#9C27B0");
        public Color CColor => Color.FromHex("#3F51B5");
        public Color DColor => Color.FromHex("#03A9F4");
        public Color EColor => Color.FromHex("#009688");
        public Color FColor => Color.FromHex("#4CAF50");
        public Color GColor => Color.FromHex("#FFEB3B");
        public Color HColor => Color.FromHex("#FF5722");

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

        public NavigationStackDebuggerViewObject StackDebugger { get; }

        public LogViewObject Log { get; }

        protected _ViewModel(INavigator navigator,
                          IViewModelDefiner vmDefiner,
                          ICacheService cacheService,
                          LogViewObject log,
                          NavigationStackDebuggerViewObject stackDebugger)
        {
            _navigator = navigator;
            _vmDefiner = vmDefiner;
            _cacheService = cacheService;
            StackDebugger = stackDebugger;
            Log = log;

            Title = GetType().Name.Replace("ViewModel", string.Empty);
        }

        protected override async Task OnNavigatedTo(object parameter)
        {
            await base.OnNavigatedTo(parameter);
            Presenters = Presenters ?? new ObservableCollection<Type>(new[] {
                typeof(SinglePagePresenter),
                typeof(StackPresenter),
                typeof(MasterDetailPresenter),
                typeof(TabbedPresenter)
            });
            SelectedPresenter = SelectedPresenter ?? Presenters?.Last();
        }
    }
}
