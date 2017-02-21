using System.Threading.Tasks;
using MVPathway.Integration.Services;
using MVPathway.Integration.Services.Contracts;
using MVPathway.Integration.Tasks.Base;
using MVPathway.MVVM.Abstractions;

namespace MVPathway.Integration.Tasks.Core
{
  public class ServiceResolutionTask : CoreIntegrationTask
  {
    private readonly IDiContainer mContainer;

    public ServiceResolutionTask(IDiContainer container)
    {
      mContainer = container;
    }

    public override async Task<bool> Execute()
    {
      mContainer.Register<IService, MyService>();
      var refA = mContainer.Resolve<IService>();
      var refB = mContainer.Resolve(typeof(IService));
      return refA != null && refA == refB;
    }
  }
}
