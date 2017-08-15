using MVPathway.Builder.Abstractions;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using MVPathway.Settings.Abstractions;
using System;

namespace MVPathway.Builder
{
    public class PathwayBuilder : IPathwayBuilder
    {
        private Action<ILogger> _loggerConfig;
        private Action<ISettingsRepository> _settingsRepositoryConfig;
        private Action<IViewModelManager> _vmManagerConfig;
        private Action<IMessenger> _messengerConfig;
        private Action<INavigator> _navigatorConfig;
        private Action<IPresenter> _presenterConfig;

        public IDiContainer Container { get; private set; }

        internal PathwayBuilder()
        {
        }

        public IPathwayBuilder UseDiContainer<TDiContainer>(Action<TDiContainer> configure = null)
          where TDiContainer : class, IDiContainer
        {
            Container = Activator.CreateInstance<TDiContainer>();
            Container.RegisterInstance(Container);

            configure?.Invoke((TDiContainer) Container);

            return this;
        }

        public IPathwayBuilder UseLogger<TLogger>(Action<TLogger> configure = null)
            where TLogger : class, ILogger
        {
            ensureDiContainerLoaded();
            Container.Register<ILogger, TLogger>();

            if (configure != null)
            {
                _loggerConfig = p => configure.Invoke(p as TLogger);
            }

            return this;
        }

        public IPathwayBuilder UseSettings<TSettingsInterface, TSettingsConcrete>(Action<TSettingsConcrete> configure = null)
            where TSettingsInterface : ISettingsRepository
            where TSettingsConcrete : class, TSettingsInterface
        {
            ensureDiContainerLoaded();
            Container.Register<TSettingsInterface, TSettingsConcrete>();

            if (configure != null)
            {
                _settingsRepositoryConfig = p => configure.Invoke(p as TSettingsConcrete);
            }

            return this;
        }

        public IPathwayBuilder UseViewModelManager<TViewModelManager>(Action<TViewModelManager> configure = null)
            where TViewModelManager : class, IViewModelManager
        {
            ensureDiContainerLoaded();
            Container.Register<IViewModelManager, TViewModelManager>();

            if (configure != null)
            {
                _vmManagerConfig = p => configure.Invoke(p as TViewModelManager);
            }

            return this;
        }

        public IPathwayBuilder UseMessenger<TMessenger>(Action<TMessenger> configure = null)
            where TMessenger : class, IMessenger
        {
            ensureDiContainerLoaded();
            Container.Register<IMessenger, TMessenger>();

            if (configure != null)
            {
                _messengerConfig = p => configure.Invoke(p as TMessenger);
            }

            return this;
        }

        public IPathwayBuilder UseNavigator<TNavigator>(Action<TNavigator> configure = null)
            where TNavigator : class, INavigator
        {
            ensureDiContainerLoaded();
            Container.Register<INavigator, TNavigator>();

            if (configure != null)
            {
                _navigatorConfig = p => configure.Invoke(p as TNavigator);
            }

            return this;
        }

        public IPathwayBuilder UsePresenter<TPresenter>(Action<TPresenter> configure = null)
            where TPresenter : class, IPresenter
        {
            ensureDiContainerLoaded();
            Container.Register<IPresenter, TPresenter>();

            if (configure != null)
            {
                _presenterConfig = p => configure.Invoke(p as TPresenter);
            }

            return this;
        }

        public IPathwayBuilder UseAppStart<TAppStart>()
            where TAppStart : class, IAppStart
        {
            ensureDiContainerLoaded();
            Container.Register<IAppStart, TAppStart>();

            return this;
        }

        public void Build()
        {
            var logger = Container.Resolve<ILogger>();
            _loggerConfig?.Invoke(logger);

            var settings = Container.Resolve<ISettingsRepository>();
            _settingsRepositoryConfig?.Invoke(settings);

            var vmManager = Container.Resolve<IViewModelManager>();
            _vmManagerConfig?.Invoke(vmManager);

            var messenger = Container.Resolve<IMessenger>();
            _messengerConfig?.Invoke(messenger);

            var presenter = Container.Resolve<IPresenter>();
            presenter.Init();
            _presenterConfig?.Invoke(presenter);

            var navigator = Container.Resolve<INavigator>();
            _navigatorConfig?.Invoke(navigator);
        }

        private void ensureDiContainerLoaded()
        {
            if (Container == null)
            {
                Container = new DiContainer();
            }
        }
    }
}
