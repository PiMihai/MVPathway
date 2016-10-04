using System;
using MVPathway.Messages.Abstractions;

namespace MVPathway.Messages.Abstractions
{
  interface IMessagingManager
  {
    void Send<TMessage>(TMessage message) where TMessage : IMessage;
    void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage;
    void Unsubscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage;
  }
}