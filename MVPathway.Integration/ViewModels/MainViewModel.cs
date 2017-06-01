using MVPathway.MVVM.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using MVPathway.Integration.Converters;
using Xamarin.Forms;
using MVPathway.Utils.ViewModels.ViewObjects;
using MVPathway.Navigation.Abstractions;

namespace MVPathway.Integration.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly INavigator _navigator;
        private readonly INavigationBus _navigationBus;

        private ObservableCollection<Type> _presenters;
        public ObservableCollection<Type> Presenters
        {
            get { return _presenters; }
            set
            {
                _presenters = value;
                OnPropertyChanged();
            }
        }

        private Type _selectedPresenter;
        public Type SelectedPresenter
        {
            get { return _selectedPresenter; }
            set
            {
                if (value == null || _selectedPresenter == value)
                {
                    return;
                }
                _selectedPresenter = value;
                OnPropertyChanged();
                ChangePresenterCommand.Execute(value);
            }
        }

        private Command<Type> _changePresenterCommand;
        public Command<Type> ChangePresenterCommand => _changePresenterCommand ?? (_changePresenterCommand = new Command<Type>(
            async (p) => await changePresenter(p)));

        private async Task changePresenter(Type presenterType)
        {
            await ((App)Application.Current).SetupWithPresenterType(presenterType);
            await _navigationBus.ChangePresenterTo(presenterType);
            await _navigator.Show<MainViewModel>();
        }

        private Command _startCommand;
        public Command StartCommand => _startCommand ??
            (_startCommand = new Command(() =>_navigator.Show<AViewModel>()));

        public NavigationStackDebuggerViewObject StackDebugger { get; private set; }

        public MainViewModel(INavigator navigator, INavigationBus navigationBus, NavigationStackDebuggerViewObject stackDebugger)
        {
            _navigator = navigator;
            _navigationBus = navigationBus;
            StackDebugger = stackDebugger;
        }

        protected override async Task OnNavigatedTo(object parameter)
        {
            await base.OnNavigatedTo(parameter);
            var presenterConverter = Application.Current.Resources["PresenterTypeListToStringConverter"] as PresenterTypeListToStringConverter;
            Presenters = presenterConverter.SupportedPresenters;
            SelectedPresenter = SelectedPresenter ?? Presenters?.FirstOrDefault();
        }
    }
}
