using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using System;
using System.Collections.Generic;

namespace MVPathway.Messages
{
    class MessagingManager : IMessagingManager
    {
        private readonly ILogger _logger;

        private readonly Dictionary<Type, List<Delegate>> mCallbackMap
          = new Dictionary<Type, List<Delegate>>();

        public MessagingManager(ILogger logger)
        {
            _logger = logger;
        }

        public void Subscribe<TMessage>(Action<TMessage> callback)
          where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!mCallbackMap.ContainsKey(messageType))
            {
                mCallbackMap[messageType] = new List<Delegate>();
            }
            var callbackList = mCallbackMap[messageType];
            if (callbackList.Contains(callback))
            {
                return;
            }
            callbackList.Add(callback);
        }

        public void Unsubscribe<TMessage>(Action<TMessage> callback)
          where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!mCallbackMap.ContainsKey(messageType))
            {
                mCallbackMap[messageType] = new List<Delegate>();
            }
            var callbackList = mCallbackMap[messageType];
            if (!callbackList.Contains(callback))
            {
                return;
            }
            callbackList.Remove(callback);
        }

        public void Send<TMessage>(TMessage message)
          where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!mCallbackMap.ContainsKey(messageType))
            {
                mCallbackMap[messageType] = new List<Delegate>();
            }
            var callbackList = mCallbackMap[messageType].ToArray();
            foreach (var callback in callbackList)
            {
                callback.DynamicInvoke(message);
            }
        }
    }
}
