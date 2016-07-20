using MVPathway.Messages.Abstractions;
using System;
using System.Collections.Generic;

namespace MVPathway.MVVM
{
  static class MessengerResolver
  {
    private static readonly Dictionary<Type, object> mMap = new Dictionary<Type, object>();

    public static IMessenger<TMessage> ResolveMessenger<TMessage>()
      where TMessage : IMessage
    {
      var messageType = typeof(TMessage);
      if (!mMap.ContainsKey(messageType))
      {
        throw new Exception($"Messenger for {messageType.FullName} not registered.");
      }
      return mMap[messageType] as IMessenger<TMessage>;
    }

    public static TMessenger RegisterMessenger<TMessenger, TMessage>()
      where TMessage : IMessage
      where TMessenger : class, IMessenger<TMessage>
    {
      var messenger = Activator.CreateInstance(typeof(TMessenger));
      mMap[typeof(TMessage)] = messenger;
      return messenger as TMessenger;
    }
  }
}
