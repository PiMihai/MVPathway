namespace MVPathway.Logging.Abstractions
{
  public interface ILogger
  {
    void LogError(string text);
    void LogWarning(string text);
    void LogInfo(string text);
  }
}
