using DryIoc;
using System;

namespace MVPathway.MVVM
{
  static class IoC
  {
    private static IContainer mContainer = new Container();

    public static void Register<T>(bool asSingleton = true)
        where T : class
    {
      var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
      mContainer.Register<T>(reuse);
    }

    public static void Register<TInterface, TConcrete>(bool asSingleton = true)
        where TConcrete : TInterface
    {
      var reuse = asSingleton ? Reuse.Singleton : Reuse.Transient;
      mContainer.Register<TInterface, TConcrete>(reuse);
    }

    public static object Resolve(Type type) => mContainer.Resolve(type);

    public static T Resolve<T>() => mContainer.Resolve<T>();
  }
}
