using MVPathway.Helpers;
using MVPathway.MVVM;
using MVPathway.Services.Contracts;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenter.Base
{
    public abstract class BasePresenter
    {
        private const string cInvalidViewModelMessage = "TViewModel is not a valid ViewModel type.";

        public virtual void Init()
        {
            MessagingCenter.Subscribe<INavigatorService, ViewModelNavigationMessage>(this, Const.CShowViewModel,
                async (sender, message) => await Show(message));
            MessagingCenter.Subscribe<INavigatorService, ViewModelNavigationMessage>(this, Const.CCloseViewModel,
                async (sender, message) => await Close(message));
        }

        protected virtual async Task<BaseViewModel> Show(ViewModelNavigationMessage message)
        {
            var viewModel = PathwayCore.Resolve(message.ViewModelType);
            if (!(viewModel is BaseViewModel))
            {
                throw new Exception(cInvalidViewModelMessage);
            }
            var baseViewModel = viewModel as BaseViewModel;
            baseViewModel.OnNavigatedTo(message.Parameter);
            return baseViewModel;
        }

        protected virtual async Task Close(ViewModelNavigationMessage message)
        {
            var viewModel = PathwayCore.Resolve(message.ViewModelType);
            if (!(viewModel is BaseViewModel))
            {
                throw new Exception(cInvalidViewModelMessage);
            }
            var baseViewModel = viewModel as BaseViewModel;
            baseViewModel.OnNavigatingFrom(message.Parameter);
        }
    }
}
