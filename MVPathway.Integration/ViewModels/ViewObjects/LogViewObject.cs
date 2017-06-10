using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using Xamarin.Forms;
using MVPathway.Integration.Messages;

namespace MVPathway.Integration.ViewModels.ViewObjects
{
    public class LogViewObject : BaseObservableObject
    {
        private ILogger _logger;
        private IMessenger _messenger;

        private FormattedString _log = new FormattedString();
        public FormattedString Log
        {
            get => _log;
            set
            {
                _log = value;
                OnPropertyChanged();
            }
        }

        public LogViewObject(ILogger logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
        }

        public void Subscribe()
        {
            _messenger.Subscribe<LogMessage>(onLogMessageReceived);
        }

        public void Unsubscribe()
        {
            _messenger.Unsubscribe<LogMessage>(onLogMessageReceived);
        }

        private void onLogMessageReceived(LogMessage message)
        {
            Log.Spans.Insert(0, message.Span);
            OnPropertyChanged(nameof(Log));
        }
    }
}
