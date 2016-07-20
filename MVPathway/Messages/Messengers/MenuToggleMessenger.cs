using MVPathway.Messages.Abstractions;
using Xamarin.Forms;

namespace MVPathway.Messages.Messengers
{
  class MenuToggleMessenger : IMessenger<MenuToggleMessage>
  {
    public const string CMessageKey = "MenuToggleMessage_MessageKey";
    
    public void Send()
    {
      MessagingCenter.Send(this, CMessageKey, new MenuToggleMessage());
    }
  }
}
