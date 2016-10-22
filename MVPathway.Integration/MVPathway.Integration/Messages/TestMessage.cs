using MVPathway.Messages.Abstractions;

namespace MVPathway.Integration.Messages
{
  class TestMessage : IMessage
  {
    public string Content { get; set; }
  }
}
