using MVPathway.Integration.Tasks.Base;
using MVPathway.MVVM.Abstractions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace MVPathway.Integration
{
  public partial class App : Application
  {
    private readonly IDiContainer mContainer;
    private bool isSandbox = false;

    public App(IDiContainer container)
    {
      InitializeComponent();
      MainPage = new ContentPage();

      mContainer = container;
    }

    protected override void OnStart()
    {
      if(isSandbox)
      {
        return;
      }

      var integrationTaskTypes = typeof(IIntegrationTask)
                .GetTypeInfo()
                .Assembly
                .DefinedTypes
                .Where(x => x.IsClass && !x.IsAbstract && typeof(IIntegrationTask).GetTypeInfo().IsAssignableFrom(x))
                .Select(x => x.AsType());

      long elapsedTotal = 0, passedTasks = 0;

      foreach (var taskType in integrationTaskTypes)
      {
        var app = PathwayCore.Create<App>();
        app.isSandbox = true;
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
          Debug.WriteLine($"Task {task.GetType().Name} has thrown {e}.");
          Debug.WriteLine(e.StackTrace);
        }
        if (result)
        {
          Debug.WriteLine($"Task {task.GetType().Name} has passed.");
          passedTasks++;
        }
        else
        {
          Debug.WriteLine($"Task {task.GetType().Name} has failed.");
        }
        stopwatch.Stop();
        Debug.WriteLine($"Took {stopwatch.ElapsedMilliseconds} ms.");
        Debug.WriteLine(string.Empty);
        elapsedTotal += stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
      }
      Debug.WriteLine("Integration tests finished.");
      Debug.WriteLine($"Took a total of {elapsedTotal} ms.");
      Debug.WriteLine($"{passedTasks}/{integrationTaskTypes.Count()} tasks were succesful.");
      Debug.WriteLine("Press any key to exit.");
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }
  }
}
