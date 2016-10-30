using MVPathway.Builder.Abstractions;
using MVPathway.Logging;
using MVPathway.Logging.Abstractions;
using MVPathway.Messages;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using MVPathway.Presenters.Abstractions;
using Xamarin.Forms;

namespace MVPathway
{
  public abstract class PathwayApplication : Application
  {
    public virtual void Configure(IPathwayBuilder builder)
    {
      builder.UseDiContainer<DiContainer>()
             .UseViewModelManager<ViewModelManager>()
             .UsePresenter<SinglePagePresenter>()
             .UseMessagingManager<MessagingManager>()
             .UseLogger<PathwayLogger>();
    }

    public abstract void Init(IDiContainer container,
                               IViewModelManager vmManager,
                               IMessagingManager messagingManager,
                               IPresenter presenter,
                               ILogger logger);
  }
}
