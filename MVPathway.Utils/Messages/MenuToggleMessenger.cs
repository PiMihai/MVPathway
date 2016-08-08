using MVPathway.Messages.Abstractions;
using Xamarin.Forms;

namespace MVPathway.Utils.Messages
{
  class MenuToggleMessenger : IMessenger<MenuToggleMessage>
  {
    public const string CMessageKey = "MenuToggleMessage_MessageKey";
    
    public void SendMessage()
    {
      MessagingCenter.Send(this, CMessageKey, new MenuToggleMessage());
    }
  }
}
