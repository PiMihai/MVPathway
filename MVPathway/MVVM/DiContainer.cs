using DryIoc;
using MVPathway.MVVM.Abstractions;
using System;

namespace MVPathway.MVVM
{
  class DiContainer : IDiContainer
  {
    private readonly IContainer mContainer = new Container();

    public void Register<T>(bool asSingleton = true)
        where T : class
    {
      var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
      mContainer.Register<T>(reuse);
    }

    public void Register<T>(T instance, bool asSingleton = true)
    {
      var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
      mContainer.RegisterInstance(instance, reuse);
    }

    public void Register<TInterface, TConcrete>(bool asSingleton = true)
        where TConcrete : TInterface
    {
      var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
      mContainer.Register<TInterface, TConcrete>(reuse);
    }

    public object Resolve(Type type) => mContainer.Resolve(type);

    public T Resolve<T>() => mContainer.Resolve<T>();
  }
}
