using MVPathway.MVVM;
using MVPathway.Presenter.Base;
using MVPathway.Services.Contracts;
using System;
using Xamarin.Forms;

namespace MVPathway
{
    public static class PathwayCore
    {
        #region Instance members

        private static INavigatorService mNavigatorService;
        private static BasePresenter mPresenter;

        #endregion

        #region Constructors

        static PathwayCore()
        {
            registerInternalServices();
            mNavigatorService = Resolve<INavigatorService>();
        }

        #endregion

        #region Public methods

        public static void SetPresenter(BasePresenter presenter)
        {
            mPresenter = presenter;
            mPresenter.Init();
        }

        public static void ShowViewModel<TViewModel>(object parameter = null)
            => mNavigatorService.Show<TViewModel>(parameter);
        public static void CloseViewModel<TViewModel>(object parameter = null)
            => mNavigatorService.Close<TViewModel>(parameter);

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

        #region Private methdos

        private static void registerInternalServices()
        {
            RegisterInterface<INavigatorService, NavigatorService>();
        }

        #endregion
    }
}
