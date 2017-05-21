using System;

namespace MVPathway.Messages.Abstractions
{
  public interface IMessenger
  {
    void Send<TMessage>(TMessage message) where TMessage : IMessage;
    void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage;
    void Unsubscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage;
  }
}