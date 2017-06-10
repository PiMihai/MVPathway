using MVPathway.Integration.Messages;
using MVPathway.Logging;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using System;
using Xamarin.Forms;

namespace MVPathway.Integration.Services
{
    public class BroadcastLogger : PathwayLogger, ILogger
    {
        private IMessenger _messenger;

        public BroadcastLogger(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public new void LogError(string text)
        {
            base.LogError(text);
            _messenger.Send(new LogMessage
            {
                Span = new Span
                {
                    Text = Environment.NewLine + text,
                    ForegroundColor = Color.Red
                }
            });
        }

        public new void LogInfo(string text)
        {
            base.LogInfo(text);
            _messenger.Send(new LogMessage
            {
                Span = new Span
                {
                    Text = Environment.NewLine + text,
                    ForegroundColor = Color.LightBlue
                }
            });
        }

        public new void LogWarning(string text)
        {
            base.LogWarning(text);
            _messenger.Send(new LogMessage
            {
                Span = new Span
                {
                    Text = Environment.NewLine + text,
                    ForegroundColor = Color.Yellow
                }
            });
        }
    }
}
