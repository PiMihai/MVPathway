namespace MVPathway.Integration.Tasks.Base
{
  public abstract class BaseIntegrationTask : IIntegrationTask
  {
    protected virtual void init()
    {

    }

    public virtual bool Execute()
    {
      init();
      return true;
    }
  }
}
