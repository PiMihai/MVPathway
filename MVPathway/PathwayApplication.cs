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
        public virtual IDiContainer Container { get; } = new DiContainer();

        public virtual void BeforeConfigure() { }

        public virtual void Configure(IPathwayBuilder builder)
        {
            builder.UseLogger<PathwayLogger>()
                   .UseSettings<ISettingsRepository, SettingsRepository>()
                   .UseViewModelManager<ViewModelManager>()
                   .UseMessenger<Messenger>()
                   .UseNavigator<Navigator>()
                   .UsePresenter<SinglePagePresenter>();
        }

        public virtual void AfterConfigure() { }
    }
}
