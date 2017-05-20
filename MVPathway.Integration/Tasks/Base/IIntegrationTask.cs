using System.Threading.Tasks;

namespace MVPathway.Integration.Tasks.Base
{
  public interface IIntegrationTask
  {
    Task<bool> Execute();
  }
}
