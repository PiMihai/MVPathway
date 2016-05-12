using MVPathway.MVVM;
using System;
using System.Threading.Tasks;

namespace MVPathway.Presenter.Base
{
    public abstract class BasePresenter
    {
        private const string cInvalidViewModelMessage = "TViewModel is not a valid ViewModel type.";

        protected internal virtual async Task<BaseViewModel> Show<TViewModel>(object parameter)
        {
            var viewModel = PathwayCore.Resolve<TViewModel>();
            if (!(viewModel is BaseViewModel))
            {
                throw new Exception(cInvalidViewModelMessage);
            }
            var baseViewModel = viewModel as BaseViewModel;
            baseViewModel.OnNavigatedTo(parameter);
            return baseViewModel;
        }

        protected internal virtual async Task Close<TViewModel>(object parameter)
        {
            var viewModel = PathwayCore.Resolve<TViewModel>();
            if (!(viewModel is BaseViewModel))
            {
                throw new Exception(cInvalidViewModelMessage);
            }
            var baseViewModel = viewModel as BaseViewModel;
            baseViewModel.OnNavigatingFrom(parameter);
        }

        protected internal abstract Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText);
    }
}
