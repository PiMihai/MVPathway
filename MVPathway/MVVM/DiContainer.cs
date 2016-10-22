using DryIoc;
using MVPathway.MVVM.Abstractions;
using System;

namespace MVPathway.MVVM
{
  class DiContainer : IDiContainer
  {
    private readonly IContainer mContainer = new Container();

    public bool IsRegistered<T>() => mContainer.IsRegistered<T>();

    public void Register(Type type, bool asSingleton = true)
    {
      if (mContainer.IsRegistered(type))
      {
        mContainer.Unregister(type);
      }
      mContainer.Register(type);
    }

    public void Register<T>(T singletonInstance) where T : class
    {
      if (mContainer.IsRegistered<T>())
      {
        mContainer.Unregister<T>();
      }
      mContainer.RegisterInstance(singletonInstance, Reuse.Singleton);
    }

    public void Register<T>(bool asSingleton = true)
        where T : class
    {
      if (mContainer.IsRegistered<T>())
      {
        mContainer.Unregister<T>();
      }
      var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
      mContainer.Register<T>(reuse);
    }

    public void Register<TInterface, TConcrete>(bool asSingleton = true)
        where TConcrete : TInterface
    {
      if (mContainer.IsRegistered<TInterface>())
      {
        mContainer.Unregister<TInterface>();
      }
      var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
      mContainer.Register<TInterface, TConcrete>(reuse);
    }

    public object Resolve(Type type) => mContainer.Resolve(type);

    public T Resolve<T>() => mContainer.Resolve<T>();
  }
}
