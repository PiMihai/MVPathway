using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using System;
using System.Threading.Tasks;

namespace MVPathway.Navigation.Abstractions
{
    public interface INavigator
    {
        bool DuringRequestedTransition { get; }

        Task ChangePresenter<TPresenter>(Action<TPresenter> configure = null)
            where TPresenter : class, IPresenter;
        Task ChangePresenter(Type presenterType, Action<IPresenter> configure = null);

        Task Show<TViewModel>(object parameter = null) where TViewModel : BaseViewModel;
        Task Show(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null);
        Task Show(BaseViewModel viewModel, object parameter = null);

        Task<ViewModelResult<TResult>> GetResult<TViewModel, TResult>(object parameter = null)
            where TViewModel : BaseResultViewModel<TResult>;
        Task<ViewModelResult<TResult>> GetResult<TResult>(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null);

        Task<ViewModelResult<TResult>> GetResult<TResult>(BaseResultViewModel<TResult> viewModel, object parameter = null);

        Task Close(object parameter = null);

        Task DisplayAlertAsync(string title, string message, string okText, string cancelText = null);
    }
}
