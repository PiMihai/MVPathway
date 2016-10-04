using MVPathway.Integration.Services;
using MVPathway.Integration.Services.Contracts;

namespace MVPathway.Integration.Tasks
{
  class ServiceResolutionTask : BaseIntegrationTask
  {
    public override bool Execute()
    {
      PathwayCore.RegisterInterface<IService, MyService>();
      var refA = PathwayCore.Resolve<IService>();
      var refB = PathwayCore.Resolve(typeof(IService));
      return refA != null && refA == refB;
    }
  }
}
