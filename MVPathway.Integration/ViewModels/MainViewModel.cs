using MVPathway.MVVM.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using MVPathway.Integration.Converters;
using Xamarin.Forms;
using MVPathway.Presenters.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;

namespace MVPathway.Integration.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IDiContainer _container;

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
                _container.Register(value);
                var instance = _container.Resolve(value) as IPresenter;
                ChangePresenterCommand.Execute(instance);
            }
        }

        private Command<IPresenter> _changePresenterCommand;
        public Command<IPresenter> ChangePresenterCommand => _changePresenterCommand ?? (_changePresenterCommand = new Command<IPresenter>(
            async (p) => await changePresenter(p)));

        private async Task changePresenter(IPresenter presenter)
        {
            _container.RegisterInstance(presenter);
            await (Application.Current as App).SetupWithPresenter(presenter);
            await presenter.Init();
            await presenter.Show<MainViewModel>();
        }

        private Command _startCommand;
        public Command StartCommand => _startCommand ??
            (_startCommand = new Command(() => _container.Resolve<IPresenter>().Show<AViewModel>()));

        public NavigationStackDebuggerViewObject StackDebugger { get; private set; }

        public MainViewModel(IDiContainer container, NavigationStackDebuggerViewObject stackDebugger)
        {
            _container = container;
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
