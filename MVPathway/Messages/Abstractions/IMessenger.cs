using System;

namespace MVPathway.Messages.Abstractions
{
  public interface IMessenger<T> where T : IMessage
  {
    void SendMessage();
  }
}
