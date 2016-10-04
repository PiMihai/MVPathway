using MVPathway.Integration.Tasks;
using System;
using System.Diagnostics;
using System.Linq;

namespace MVPathway.Integration
{
  static class Program
  {
    static void Main(string[] args)
    {
      var integrationTasks = typeof(IIntegrationTask)
          .Assembly
          .DefinedTypes
          .Where(x => x.IsClass && typeof(IIntegrationTask).IsAssignableFrom(x))
          .Select(x => Activator.CreateInstance(x) as IIntegrationTask);

      long elapsedTotal = 0;

      Console.ForegroundColor = ConsoleColor.White;

      foreach (var task in integrationTasks)
      {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        bool result = false;
        try
        {
          result = task.Execute();
        }
        catch (Exception e)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine($"Task {task.GetType().Name} has thrown {e}.");
          Console.WriteLine(e.StackTrace);
          Console.ForegroundColor = ConsoleColor.White;
        }
        if (result)
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine($"Task {task.GetType().Name} has passed.");
          Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine($"Task {task.GetType().Name} has failed.");
          Console.ForegroundColor = ConsoleColor.White;
        }
        stopwatch.Stop();
        Console.WriteLine($"Took {stopwatch.ElapsedMilliseconds} ms.");
        Console.WriteLine();
        elapsedTotal += stopwatch.ElapsedMilliseconds;
        stopwatch.Reset();
      }
      Console.WriteLine("Integration tests finished.");
      Console.WriteLine($"Took a total of {elapsedTotal} ms.");
      Console.WriteLine("Press any key to exit.");
      Console.ReadKey();
    }
  }
}
