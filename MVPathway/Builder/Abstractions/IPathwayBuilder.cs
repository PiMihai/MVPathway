using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using MVPathway.Settings.Abstractions;
using System;

namespace MVPathway.Builder.Abstractions
{
    public interface IPathwayBuilder
    {
        IPathwayBuilder UseMessenger<TMessenger>(Action<TMessenger> configure = null)
            where TMessenger : class, IMessenger;
        IPathwayBuilder UseLogger<TLogger>(Action<TLogger> configure = null)
            where TLogger : class, ILogger;
        IPathwayBuilder UseSettings<TSettingsInterface, TSettingsConcrete>(Action<TSettingsConcrete> configure = null)
            where TSettingsInterface : ISettingsRepository
            where TSettingsConcrete : class, TSettingsInterface;
        IPathwayBuilder UseNavigator<TNavigator>(Action<TNavigator> configure = null)
            where TNavigator : class, INavigator;
        IPathwayBuilder UsePresenter<TPresenter>(Action<TPresenter> configure = null)
            where TPresenter : class, IPresenter;
        IPathwayBuilder UseViewModelManager<TViewModelManager>(Action<TViewModelManager> configure = null)
            where TViewModelManager : class, IViewModelManager;
        IPathwayBuilder UseAppStart<TAppStart>()
            where TAppStart : class, IAppStart;
    }
}
