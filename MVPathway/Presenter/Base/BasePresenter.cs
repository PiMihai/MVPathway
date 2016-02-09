using MVPathway.Helpers;
using MVPathway.MVVM;
using System;
using Xamarin.Forms;

namespace MVPathway.Presenter.Base
{
    public abstract class BasePresenter : IPresenter
    {
        private const string cInvalidViewModelMessage = "TViewModel is not a valid ViewModel type.";

        public BasePresenter()
        {
            MessagingCenter.Subscribe<BaseViewModel, Type>(this, Const.CShowViewModel,
                (sender, viewModelType) => Show(viewModelType));
            MessagingCenter.Subscribe<BaseViewModel, Type>(this, Const.CCloseViewModel,
                (sender, viewModelType) => Close(viewModelType));
        }

        public virtual BaseViewModel Show(Type viewModelType)
        {
            var viewModel = MVPCore.Resolve(viewModelType);
            if (!(viewModel is BaseViewModel))
            {
                throw new Exception(cInvalidViewModelMessage);
            }
            var baseViewModel = viewModel as BaseViewModel;
            baseViewModel.OnAppearing();
            return baseViewModel;
        }

        public virtual void Close(Type viewModelType)
        {
            var viewModel = MVPCore.Resolve(viewModelType);
            if (!(viewModel is BaseViewModel))
            {
                throw new Exception(cInvalidViewModelMessage);
            }
            (viewModel as BaseViewModel).OnDisappearing();
        }
    }
}
