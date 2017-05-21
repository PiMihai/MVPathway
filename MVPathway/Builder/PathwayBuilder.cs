using MVPathway.Builder.Abstractions;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters.Abstractions;
using System;

namespace MVPathway.Builder
{
    public class PathwayBuilder : IPathwayBuilder
    {
        public IDiContainer Container { get; private set; }

        internal PathwayBuilder()
        {
        }

        internal Action<IPresenter> ConfigurePresenter { get; private set; }

        public IPathwayBuilder UseDiContainer<TDiContainer>()
          where TDiContainer : class, IDiContainer
        {
            Container = Activator.CreateInstance<TDiContainer>();
            Container.RegisterInstance(Container);

            return this;
        }

        public IPathwayBuilder UseLogger<TLogger>()
            where TLogger : class, ILogger
        {
            ensureDiContainerLoaded();
            Container.Register<ILogger, TLogger>();

            return this;
        }

        public IPathwayBuilder UseMessagingManager<TMessagingManager>()
            where TMessagingManager : class, IMessenger
        {
            ensureDiContainerLoaded();
            Container.Register<IMessenger, TMessagingManager>();

            return this;
        }

        public IPathwayBuilder UsePresenter<TPresenter>(Action<TPresenter> configure = null)
            where TPresenter : class, IPresenter
        {
            ensureDiContainerLoaded();
            Container.Register<IPresenter, TPresenter>();
            ConfigurePresenter = p => configure?.Invoke(p as TPresenter);

            return this;
        }

        public IPathwayBuilder UseViewModelManager<TViewModelManager>()
            where TViewModelManager : class, IViewModelManager
        {
            ensureDiContainerLoaded();
            Container.Register<IViewModelManager, TViewModelManager>();

            return this;
        }

        private void ensureDiContainerLoaded()
        {
            if (Container == null)
            {
                Container = new DiContainer();
            }
        }

        public IPathwayBuilder UseNavigator<TNavigator>()
            where TNavigator : class, INavigator
        {
            ensureDiContainerLoaded();
            Container.Register<INavigator, TNavigator>();

            return this;
        }

        public IPathwayBuilder UseNavigationBus<TNavigationBus>()
            where TNavigationBus : class, INavigationBus
        {
            ensureDiContainerLoaded();
            Container.Register<INavigationBus, TNavigationBus>();

            return this;
        }
    }
}
