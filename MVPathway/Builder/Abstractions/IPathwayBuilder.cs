using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters.Abstractions;

namespace MVPathway.Builder.Abstractions
{
    public interface IPathwayBuilder
    {
        IDiContainer Container { get; }
        IPathwayBuilder UseDiContainer<TDiContainer>() where TDiContainer : IDiContainer;
        IPathwayBuilder UseMessagingManager<TMessagingManager>() where TMessagingManager : IMessagingManager;
        IPathwayBuilder UseLogger<TLogger>() where TLogger : ILogger;
        IPathwayBuilder UsePresenter<TPresenter>() where TPresenter : IPresenter;
        IPathwayBuilder UseViewModelManager<TViewModelManager>() where TViewModelManager : IViewModelManager;
    }
}
