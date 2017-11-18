using System;
using System.Threading.Tasks;
using MVPathway.Integration.Services.Contracts;
using MVPathway.Builder;
using System.Diagnostics;
using System.Reflection;
using MVPathway.Logging.Abstractions;
using System.Linq;

namespace MVPathway.Integration.Services
{
    public class TaskRunner : ITaskRunner
    {
        private readonly ILogger _logger;

        public TaskRunner(ILogger logger)
        {
            _logger = logger;
        }

        public async Task RunAllTasks()
        {
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
                app.Container.Register(taskType);

                var task = app.Container.Resolve(taskType) as IIntegrationTask;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = false;
                try
                {
                    result = await task.Execute();
                }
                catch (Exception e)
                {
                    _logger.LogInfo($"Task {task.GetType().Name} has thrown {e}.");
                    _logger.LogInfo(e.StackTrace);
                }
                if (result)
                {
                    _logger.LogInfo($"Task {task.GetType().Name} has passed.");
                    passedTasks++;
                }
                else
                {
                    _logger.LogInfo($"Task {task.GetType().Name} has failed.");
                }
                stopwatch.Stop();
                _logger.LogInfo($"Took {stopwatch.ElapsedMilliseconds} ms.");
                _logger.LogInfo(string.Empty);
                elapsedTotal += stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
            }
            _logger.LogInfo("Integration tests finished.");
            _logger.LogInfo($"{passedTasks}/{integrationTaskTypes.Count()} tasks were succesful.");
            _logger.LogInfo($"Took a total of {elapsedTotal} ms.");
        }
    }
}
