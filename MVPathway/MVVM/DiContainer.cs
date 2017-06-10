using DryIoc;
using MVPathway.MVVM.Abstractions;
using System;

namespace MVPathway.MVVM
{
    public class DiContainer : IDiContainer
    {
        private readonly IContainer _container = new Container();

        public bool IsRegistered<T>() => _container.IsRegistered<T>();

        public void Register(Type type, bool asSingleton = true)
        {
            if (_container.IsRegistered(type))
            {
                _container.Unregister(type);
            }
            _container.Register(type, asSingleton ? Reuse.Singleton : Reuse.Transient);
        }

        public void RegisterInstance<T>(T singletonInstance) where T : class
        {
            if (_container.IsRegistered<T>())
            {
                _container.Unregister<T>();
            }
            _container.RegisterInstance(singletonInstance, Reuse.Singleton);
        }

        public void Register<T>(bool asSingleton = true)
            where T : class
        {
            if (_container.IsRegistered<T>())
            {
                _container.Unregister<T>();
            }
            var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
            _container.Register<T>(reuse);
        }

        public void Register<TInterface, TConcrete>(bool asSingleton = true)
            where TConcrete : TInterface
        {
            if (_container.IsRegistered<TInterface>())
            {
                _container.Unregister<TInterface>();
            }
            var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
            _container.Register<TInterface, TConcrete>(reuse);
        }

        public object Resolve(Type type) => _container.Resolve(type);

        public T Resolve<T>() => _container.Resolve<T>();
    }
}
