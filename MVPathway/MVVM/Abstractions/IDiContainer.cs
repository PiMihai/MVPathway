using System;

namespace MVPathway.MVVM.Abstractions
{
  interface IDiContainer
  {
    void Register<T>(bool asSingleton = true) where T : class;
    void Register<TInterface, TConcrete>(bool asSingleton = true) where TConcrete : TInterface;
    object Resolve(Type type);
    T Resolve<T>();
  }
}