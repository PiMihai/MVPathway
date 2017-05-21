using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using System;

namespace MVPathway.Builder.Abstractions
{
    public interface IPathwayBuilder
    {
        IDiContainer Container { get; }
        IPathwayBuilder UseDiContainer<TDiContainer>()
            where TDiContainer : class, IDiContainer;
        IPathwayBuilder UseNavigator<TNavigator>()
            where TNavigator : class, INavigator;
        IPathwayBuilder UseNavigationBus<TNavigationBus>()
            where TNavigationBus : class, INavigationBus;
        IPathwayBuilder UseMessagingManager<TMessagingManager>()
            where TMessagingManager : class, IMessenger;
        IPathwayBuilder UseLogger<TLogger>()
            where TLogger : class, ILogger;
        IPathwayBuilder UsePresenter<TPresenter>(Action<TPresenter> configure = null)
            where TPresenter : class, IPresenter;
        IPathwayBuilder UseViewModelManager<TViewModelManager>()
            where TViewModelManager : class, IViewModelManager;
    }
}
