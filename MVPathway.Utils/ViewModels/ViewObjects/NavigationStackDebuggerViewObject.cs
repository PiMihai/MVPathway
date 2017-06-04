using MVPathway.Messages;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVPathway.Utils.ViewModels.ViewObjects
{
    public class NavigationStackDebuggerViewObject : BaseObservableObject
    {
        private readonly IMessenger _messenger;
        public ObservableCollection<string> NavigationStack { get; private set; }
            = new ObservableCollection<string>();

        public NavigationStackDebuggerViewObject(IMessenger messenger)
        {
            _messenger = messenger;
        }

        public void Subscribe()
        {
            _messenger.Subscribe<NavigationStackUpdatedMessage>(onStackUpdated);
        }

        public void Unsubscribe()
        {
            _messenger.Unsubscribe<NavigationStackUpdatedMessage>(onStackUpdated);
        }

        private void onStackUpdated(NavigationStackUpdatedMessage message)
        {
            if (message.WasPopped)
            {
                NavigationStack.Remove(message.ViewModel.GetType().Name.Replace("ViewModel", string.Empty));
            }
            else
            {
                NavigationStack.Add(message.ViewModel.GetType().Name.Replace("ViewModel", string.Empty));
            }
        }
    }
}
