using MVPathway.Messages;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;
using System.Collections.ObjectModel;
using System.Linq;

namespace MVPathway.Utils.ViewModels.ViewObjects
{
    public class NavigationStackDebuggerViewObject : BaseObservableObject
    {
        private IDiContainer _container;

        public ObservableCollection<string> NavigationStack =>
            new ObservableCollection<string>(_container.Resolve<IPresenter>()
                .NavigationStack.Select(vm => vm.GetType().Name));

        public NavigationStackDebuggerViewObject(IDiContainer container, IMessagingManager messenger)
        {
            _container = container;
            messenger.Subscribe<NavigationStackUpdatedMessage>(
                m => OnPropertyChanged(nameof(NavigationStack)));
        }
    }
}
