using MVPathway.Integration.Messages;
using MVPathway.Integration.Tasks.Base;
using MVPathway.Messages.Abstractions;
using System;
using System.Threading.Tasks;

namespace MVPathway.Integration.Tasks.Core
{
  public class MessagingSubscriptionTask : CoreIntegrationTask
  {
    private readonly IMessagingManager mMessagingManager;

    public MessagingSubscriptionTask(IMessagingManager messagingManager)
    {
      mMessagingManager = messagingManager;
    }

    public override async Task<bool> Execute()
    {
      await base.Execute();

      var received = false;
      var content = "Foo";
      Action<TestMessage> callback = m => received = m.Content == content;
      var message = new TestMessage { Content = content };
      mMessagingManager.Subscribe(callback);
      mMessagingManager.Send(message);
      if(!received)
      {
        return false;
      }
      received = false;
      mMessagingManager.Unsubscribe(callback);
      mMessagingManager.Send(message);
      return !received;
    }
  }
}
