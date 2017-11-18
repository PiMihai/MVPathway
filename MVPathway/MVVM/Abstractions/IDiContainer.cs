using System;

namespace MVPathway.MVVM.Abstractions
{
    public interface IDiContainer
    {
        bool IsRegistered<T>();
        bool IsRegistered(Type type);
        void Register<T>(bool asSingleton = true) where T : class;
        void RegisterInstance<T>(T singletonInstance) where T : class;
        void Register(Type type, bool asSingleton = true);
        void Register<TInterface, TConcrete>(bool asSingleton = true) where TConcrete : TInterface;
        object Resolve(Type type);
        T Resolve<T>();
    }
}