namespace MVPathway.Integration.Tasks
{
  class BaseIntegrationTask : IIntegrationTask
  {
    protected IPathwayCore PathwayCore { get; private set; }

    public BaseIntegrationTask()
    {
      PathwayCore = PathwayCoreFactory.Create();
    }

    public virtual bool Execute()
    {
      return true;
    }
  }
}
