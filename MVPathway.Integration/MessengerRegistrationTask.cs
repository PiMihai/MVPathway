using MVPathway.Messages;
using MVPathway.Presenters;

namespace MVPathway.Integration
{
  class MessengerRegistrationTask : IIntegrationTask
  {
    public bool Execute()
    {
      PathwayCore.SetPresenter(new NavigableMasterDetailPresenter());
      var messenger = PathwayCore.ResolveMessenger<MenuToggleMessage>();
      return messenger != null;
    }
  }
}
