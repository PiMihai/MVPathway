using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using System;
using System.Threading.Tasks;

namespace MVPathway.Navigation.Abstractions
{
    public interface INavigator
    {
        Task ChangePresenter<TPresenter>()
            where TPresenter : IPresenter;
        Task ChangePresenter(Type presenterType);

        Task<TViewModel> Show<TViewModel>(object parameter = null) where TViewModel : BaseViewModel;
        Task<BaseViewModel> Show(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null);
        Task<BaseViewModel> Show(BaseViewModel viewModel, object parameter = null);

        Task<ViewModelResult<TResult>> GetResult<TViewModel, TResult>(object parameter = null)
            where TViewModel : BaseResultViewModel<TResult>;
        Task<ViewModelResult<TResult>> GetResult<TResult>(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null);

        Task<ViewModelResult<TResult>> GetResult<TResult>(BaseResultViewModel<TResult> viewModel, object parameter = null);

        Task<BaseViewModel> Close(object parameter = null);

        Task DisplayAlertAsync(string title, string message, string okText, string cancelText = null);
    }
}
