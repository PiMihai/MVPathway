using MVPathway.Helpers;
using MVPathway.MVVM;
using MVPathway.Presenter.Base;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway
{
    public static class PathwayCore
    {
        #region Instance members
        
        private static BasePresenter mPresenter;

        #endregion

        #region Public methods

        public static void SetPresenter(BasePresenter presenter)
        {
            mPresenter = presenter;
        }

        public static async Task ShowViewModelAsync<TViewModel>(object parameter = null)
        {
            await mPresenter.Show<TViewModel>(parameter);
        }

        public static async Task CloseViewModel<TViewModel>(object parameter = null)
        {
            await mPresenter.Close<TViewModel>(parameter);
        }

        public static async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
        {
            return await mPresenter.DisplayAlertAsync(title, message, okText, cancelText);
        }

        public static void RegisterInterface<TInterface,TConcrete>(bool asSingleton = true)
            where TConcrete : class
            => IoC.Register<TInterface, TConcrete>(asSingleton);

        public static void RegisterType<T>(bool asSingleton = true)
            where T : class
            => IoC.Register<T>(asSingleton);

        public static T Resolve<T>() => IoC.Resolve<T>();
        public static object Resolve(Type type) => IoC.Resolve(type);

        public static void RegisterPage<TViewModel, TPage>()
            where TViewModel : class
            where TPage : class
        {
            IoC.Register<TViewModel>();
            PageFactory.RegisterPageForViewModel<TViewModel, TPage>();
        }

        public static Page GetPageForViewModel(BaseViewModel viewModel)
            => PageFactory.GetPageForViewModel(viewModel);

        #endregion
    }
}
