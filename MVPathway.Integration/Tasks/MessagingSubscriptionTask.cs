using MVPathway.Integration.Messages;
using System;

namespace MVPathway.Integration.Tasks
{
  class MessagingSubscriptionTask : BaseIntegrationTask
  {
    public override bool Execute()
    {
      var received = false;
      var content = "Foo";
      Action<TestMessage> callback = m => received = m.Content == content;
      var message = new TestMessage { Content = content };
      PathwayCore.SubscribeToMessage(callback);
      PathwayCore.SendMessage(message);
      if(!received)
      {
        return false;
      }
      received = false;
      PathwayCore.UnsubscribeToMessage(callback);
      PathwayCore.SendMessage(message);
      return !received;
    }
  }
}
