using MVPathway.Builder.Abstractions;
using MVPathway.Logging;
using MVPathway.Messages;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation;
using MVPathway.Presenters;
using MVPathway.Settings;
using MVPathway.Settings.Abstractions;
using Xamarin.Forms;

namespace MVPathway
{
    public abstract class PathwayApplication : Application
    {
        public virtual void Configure(IPathwayBuilder builder)
        {
            builder.UseDiContainer<DiContainer>()
                   .UseLogger<PathwayLogger>()
                   .UseSettings<ISettingsRepository, SettingsRepository>()
                   .UseViewModelManager<ViewModelManager>()
                   .UseMessenger<Messenger>()
                   .UseNavigator<Navigator>()
                   .UsePresenter<SinglePagePresenter>();
        }

        public abstract void ConfigureViewModels(IViewModelManager vmManager);

        public virtual void ConfigureServices(IDiContainer container) { }
    }
}
