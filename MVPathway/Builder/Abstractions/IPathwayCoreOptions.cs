using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;

namespace MVPathway.Builder.Abstractions
{
  public interface IPathwayCoreOptions
  {
    void UseMessagingManager<TMessagingManager>() where TMessagingManager : IMessagingManager;
    void UseLogger<TLogger>() where TLogger : ILogger;
    void UsePresenter<TPresenter>() where TPresenter : IPresenter;
    void UseViewModelManager<TViewModelManager>() where TViewModelManager : IViewModelManager;
  }
}
