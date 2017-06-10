using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using System;
using System.Collections.Generic;

namespace MVPathway.Messages
{
    public class Messenger : IMessenger
    {
        private readonly ILogger _logger;

        private readonly Dictionary<Type, List<Delegate>> _callbackMap
          = new Dictionary<Type, List<Delegate>>();

        public Messenger(ILogger logger)
        {
            _logger = logger;
        }

        public void Subscribe<TMessage>(Action<TMessage> callback)
          where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (!_callbackMap.ContainsKey(messageType))
            {
                _callbackMap[messageType] = new List<Delegate>();
            }
            var callbackList = _callbackMap[messageType];
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
            if (!_callbackMap.ContainsKey(messageType))
            {
                _callbackMap[messageType] = new List<Delegate>();
            }
            var callbackList = _callbackMap[messageType];
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
            if (!_callbackMap.ContainsKey(messageType))
            {
                _callbackMap[messageType] = new List<Delegate>();
            }
            var callbackList = _callbackMap[messageType].ToArray();
            foreach (var callback in callbackList)
            {
                callback.DynamicInvoke(message);
            }
        }
    }
}
