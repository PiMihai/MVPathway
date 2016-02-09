using MVPathway.MVVM;
using MVPathway.Presenter;
using System;

namespace MVPathway
{
    public static class PathwayCore
    {
        private static IPresenter mPresenter;

        public static void SetPresenter(IPresenter presenter)
        {
            mPresenter = presenter;
        }

        public static void RegisterInterface<TInterface,TConcrete>(bool asSingleton = true)
            where TConcrete : class
        {
            IoC.Register<TInterface, TConcrete>(asSingleton);
        }

        public static void RegisterType<T>(bool asSingleton = true)
            where T : class
        {
            IoC.Register<T>(asSingleton);
        }

        public static T Resolve<T>()
        {
            return IoC.Resolve<T>();
        }

        public static object Resolve(Type type)
        {
            return IoC.Resolve(type);
        }

        public static void RegisterPage<TViewModel, TPage>()
            where TViewModel : class
            where TPage : class
        {
            IoC.Register<TViewModel>();
            PageFactory.RegisterPageForViewModel<TViewModel, TPage>();
        }
    }
}
