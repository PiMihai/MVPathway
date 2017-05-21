using MVPathway.Integration.Tasks.Base;
using MVPathway.Integration.Tasks.Messages;
using MVPathway.Messages.Abstractions;
using System;
using System.Threading.Tasks;

namespace MVPathway.Integration.Tasks.Core
{
    public class MessagingSubscriptionTask : CoreIntegrationTask
    {
        private readonly IMessenger _messenger;

        public MessagingSubscriptionTask(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public override async Task<bool> Execute()
        {
            await base.Execute();

            var received = false;
            var content = "Foo";
            Action<TestMessage> callback = m => received = m.Content == content;
            var message = new TestMessage { Content = content };
            _messenger.Subscribe(callback);
            _messenger.Send(message);
            if (!received)
            {
                return false;
            }
            received = false;
            _messenger.Unsubscribe(callback);
            _messenger.Send(message);
            return !received;
        }
    }
}
