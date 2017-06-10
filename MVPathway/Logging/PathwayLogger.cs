using MVPathway.Logging.Abstractions;
using System.Diagnostics;

namespace MVPathway.Logging
{
    public class PathwayLogger : ILogger
    {
        public void LogError(string text) => Debug.WriteLine($"MVPathway ERROR : {text}");

        public void LogInfo(string text) => Debug.WriteLine($"MVPathway INFO : {text}");

        public void LogWarning(string text) => Debug.WriteLine($"MVPathway WARNING : {text}");
    }
}
