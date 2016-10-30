using MVPathway.Builder;
using MVPathway.Integration.Tasks.Base;
using MVPathway.MVVM.Abstractions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.Presenters.Abstractions;
using Xamarin.Forms;

namespace MVPathway.Integration
{
  public partial class App : PathwayApplication
  {
    private IDiContainer mContainer;
    private ILogger mLogger;

    public override void Init(IDiContainer container,
                               IViewModelManager vmManager,
                               IMessagingManager messagingManager,
                               IPresenter presenter,
                               ILogger logger)
    {
      mContainer = container;
      mLogger = logger;

      MainPage = new ContentPage();
    }

    protected override void OnStart()
    {
      base.OnStart();

      var integrationTaskTypes = typeof(IIntegrationTask)
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IIntegrationTask).GetTypeInfo().IsAssignableFrom(x))
                .Select(x => x.AsType());

      long elapsedTotal = 0, passedTasks = 0;

      foreach (var taskType in integrationTaskTypes)
      {
        var app = PathwayFactory.Create<App>();
        app.mContainer.Register(taskType, true);

        var task = app.mContainer.Resolve(taskType) as IIntegrationTask;
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        bool result = false;
        try
        {
          result = task.Execute();
        }
        catch (Exception e)
        {
          mLogger.LogInfo($"Task {task.GetType().Name} has thrown {e}.");
          mLogger.LogInfo(e.StackTrace);
        }
        if (result)
        {
          mLogger.LogInfo($"Task {task.GetType().Name} has passed.");
          passedTasks++;
        }
        else
        {
          mLogger.LogInfo($"Task {task.GetType().Name} has failed.");
        }
        stopwatch.Stop();
        mLogger.LogInfo($"Took {stopwatch.ElapsedMilliseconds} ms.");
        mLogger.LogInfo(string.Empty);
        elapsedTotal += stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
      }
      mLogger.LogInfo("Integration tests finished.");
      mLogger.LogInfo($"{passedTasks}/{integrationTaskTypes.Count()} tasks were succesful.");
      mLogger.LogInfo($"Took a total of {elapsedTotal} ms.");
      mLogger.LogInfo("Press any key to exit.");
    }
  }
}
