using MVPathway.Builder;
using MVPathway.Builder.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;

namespace MVPathway.Utils.Builder
{
    public static class PathwayBuilderExtensions
    {
        public static IPathwayBuilder UseNavigationStackDebugger(this IPathwayBuilder builder)
        {
            builder.Container.Register<NavigationStackDebuggerViewObject>();
            return builder;
        }
    }
}
