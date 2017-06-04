using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;

namespace MVPathway.Messages
{
    public class NavigationStackUpdatedMessage : IMessage
    {
        public bool WasPopped { get; set; }
        public BaseViewModel ViewModel { get; set; }
    }
}
