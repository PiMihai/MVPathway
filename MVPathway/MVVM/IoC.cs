using Autofac;
using System;

namespace MVPathway.MVVM
{
    static class IoC
    {
        private static IContainer mContainer = new ContainerBuilder().Build();

        public static void Register<T>(bool asSingleton = true)
            where T : class
        {
            var builder = new ContainerBuilder();
            var registration = builder.RegisterType<T>();
            if (asSingleton)
            {
                registration.SingleInstance();
            }
            builder.Update(mContainer);
        }

        public static void Register<TInterface,TConcrete>(bool asSingleton = true)
            where TConcrete : class
        {
            var builder = new ContainerBuilder();
            var registration = builder.RegisterType<TConcrete>().As<TInterface>();
            if (asSingleton)
            {
                registration.SingleInstance();
            }
            builder.Update(mContainer);
        }

        public static object Resolve(Type type)
        {
            return mContainer.Resolve(type);
        }

        public static T Resolve<T>()
        {
            return mContainer.Resolve<T>();
        }
    }
}
