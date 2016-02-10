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

        public BasePresenter()
        {
            MessagingCenter.Subscribe<INavigatorService, Type>(this, Const.CShowViewModel,
                async (sender, viewModelType) => await Show(viewModelType));
            MessagingCenter.Subscribe<INavigatorService, Type>(this, Const.CCloseViewModel,
                async (sender, viewModelType) => await Close(viewModelType));
        }

        protected virtual async Task<BaseViewModel> Show(Type viewModelType)
        {
            var viewModel = PathwayCore.Resolve(viewModelType);
            if (!(viewModel is BaseViewModel))
            {
                throw new Exception(cInvalidViewModelMessage);
            }
            var baseViewModel = viewModel as BaseViewModel;
            baseViewModel.OnAppearing();
            return baseViewModel;
        }

        protected virtual async Task Close(Type viewModelType)
        {
            var viewModel = PathwayCore.Resolve(viewModelType);
            if (!(viewModel is BaseViewModel))
            {
                throw new Exception(cInvalidViewModelMessage);
            }
            (viewModel as BaseViewModel).OnDisappearing();
        }
    }
}
