using MVPathway.Messages.Abstractions;

namespace MVPathway.Integration.Tasks.Messages
{
    class TestMessage : IMessage
    {
        public string Content { get; set; }
    }
}
