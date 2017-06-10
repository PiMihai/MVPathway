using MVPathway.Builder;
using MVPathway.Builder.Abstractions;
using MVPathway.Integration.Services;
using MVPathway.Integration.ViewModels.ViewObjects;
using MVPathway.Logging.Abstractions;

namespace MVPathway.Integration.Builder
{
    public static class PathwayBuilderExtensions
    {
        public static IPathwayBuilder UseLogViewer(this IPathwayBuilder builder)
        {
            var b = builder as PathwayBuilder;

            b.Container.Register<ILogger, BroadcastLogger>();
            b.Container.Register<LogViewObject>();
            b.Container.Resolve<LogViewObject>().Subscribe();

            return builder;
        }
    }
}
