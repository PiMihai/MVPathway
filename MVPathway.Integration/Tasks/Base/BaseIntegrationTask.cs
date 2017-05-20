using System.Threading.Tasks;

namespace MVPathway.Integration.Tasks.Base
{
  public abstract class BaseIntegrationTask : IIntegrationTask
  {
    public virtual Task<bool> Execute()
    {
      return Task.FromResult(true);
    }
  }
}
