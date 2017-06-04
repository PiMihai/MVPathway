using MVPathway.Builder;
using MVPathway.Builder.Abstractions;
using MVPathway.Utils.ViewModels.ViewObjects;

namespace MVPathway.Utils.Builder
{
    public static class PathwayBuilderExtensions
    {
        public static IPathwayBuilder UseNavigationStackDebugger(this IPathwayBuilder builder)
        {
            var b = builder as PathwayBuilder;

            b.Container.Register<NavigationStackDebuggerViewObject>();
            b.Container.Resolve<NavigationStackDebuggerViewObject>().Subscribe();

            return builder;
        }
    }
}
