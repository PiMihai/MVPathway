using System.Threading.Tasks;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters;
using Xamarin.Forms;
using System.Collections.Generic;
using System;
using System.Linq;
using MVPathway.Builder.Abstractions;
using MVPathway.Builder;
using MVPathway.Presenters.Abstractions;

namespace MVPathway.Roam
{
    public interface IRoamState
    {
        Type ViewModelType { get; }
        Type PageType { get; }
    }

    public class RoamState<TViewModel, TPage> : IRoamState
        where TViewModel : BaseViewModel
        where TPage : Page
    {
        public Type ViewModelType => typeof(TViewModel);
        public Type PageType => typeof(TPage);
    }

    public interface IRoamKey
    {
        IRoamState From { get; }
        IRoamState To { get; }
    }

    public class RoamKey<TPreviousViewModel, TPreviousPage, TNextViewModel, TNextPage>
        : IRoamKey
        where TPreviousViewModel : BaseViewModel
        where TPreviousPage : Page
        where TNextViewModel : BaseViewModel
        where TNextPage : Page
    {
        IRoamState IRoamKey.From { get; } = new RoamState<TPreviousViewModel, TPreviousPage>();
        IRoamState IRoamKey.To { get; } = new RoamState<TNextViewModel, TNextPage>();
    }

    public interface IRoamBuilder
    {
        IRoamBuilderSourceExtension<TSourceViewModel> From<TSourceViewModel>()
            where TSourceViewModel : BaseViewModel;
    }

    public interface IRoamBuilderSourceExtension<TSourceViewModel>
        where TSourceViewModel : BaseViewModel
    {
        IRoamBuilderPageAwareSourceExtension<TSourceViewModel, TSourcePage> With<TSourcePage>()
            where TSourcePage : Page;
    }

    public interface IRoamBuilderPageAwareSourceExtension<TSourceViewModel, TSourcePage>
        where TSourceViewModel : BaseViewModel
        where TSourcePage : Page
    {
        IRoamBuilderTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel> To<TTargetViewModel>()
            where TTargetViewModel : BaseViewModel;
    }

    public interface IRoamBuilderTargetExtension<
        TSourceViewModel,
        TSourcePage,
        TTargetViewModel>
        where TSourceViewModel : BaseViewModel
        where TSourcePage : Page
        where TTargetViewModel : BaseViewModel
    {
        IRoamBuilderPageAwareTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel, TTargetPage> With<TTargetPage>()
            where TTargetPage : Page;
    }

    public interface IRoamBuilderPageAwareTargetExtension<
        TSourceViewModel,
        TSourcePage,
        TTargetViewModel,
        TTargetPage>
        where TSourceViewModel : BaseViewModel
        where TSourcePage : Page
        where TTargetViewModel : BaseViewModel
        where TTargetPage : Page
    {
        IRoamBuilder Do(Func<TSourcePage, TTargetPage, Task> presenter);
    }

    internal class RoamBuilder : IRoamBuilder
    {
        private readonly IViewModelManager _vmManager;

        internal Dictionary<IRoamKey, Func<Page, Page, Task>> Delegates { get; }
            = new Dictionary<IRoamKey, Func<Page, Page, Task>>();

        internal RoamPresenter Presenter { get; private set; }

        public RoamBuilder(RoamPresenter presenter, IViewModelManager vmManager)
        {
            Presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _vmManager = vmManager;
        }

        public IRoamBuilderSourceExtension<TSourceViewModel> From<TSourceViewModel>() where TSourceViewModel : BaseViewModel
            => new RoamBuilderSourceExtension<TSourceViewModel>(this);

        public void Build()
        {
            foreach (var key in Delegates.Keys)
            {
                _vmManager.RegisterPageForViewModel(key.From.ViewModelType, key.From.PageType);
                _vmManager.RegisterPageForViewModel(key.To.ViewModelType, key.To.PageType);

                Presenter.Delegates.Add(key, Delegates[key]);
            }
        }
    }

    internal class RoamBuilderSourceExtension<TSourceViewModel>
        : IRoamBuilderSourceExtension<TSourceViewModel>
        where TSourceViewModel : BaseViewModel
    {
        private readonly RoamBuilder _builder;

        public RoamBuilderSourceExtension(RoamBuilder builder)
        {
            _builder = builder;
        }

        public IRoamBuilderPageAwareSourceExtension<TSourceViewModel, TSourcePage> With<TSourcePage>() where TSourcePage : Page
            => new RoamBuilderPageAwareSourceExtension<TSourceViewModel, TSourcePage>(_builder);
    }

    internal class RoamBuilderPageAwareSourceExtension<TSourceViewModel, TSourcePage>
        : IRoamBuilderPageAwareSourceExtension<TSourceViewModel, TSourcePage>
        where TSourceViewModel : BaseViewModel
        where TSourcePage : Page
    {
        private readonly RoamBuilder _builder;

        public RoamBuilderPageAwareSourceExtension(RoamBuilder builder)
        {
            _builder = builder;
        }

        public IRoamBuilderTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel> To<TTargetViewModel>() where TTargetViewModel : BaseViewModel
            => new RoamBuilderTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel>(_builder);
    }

    internal class RoamBuilderTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel>
        : IRoamBuilderTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel>
        where TSourceViewModel : BaseViewModel
        where TSourcePage : Page
        where TTargetViewModel : BaseViewModel
    {
        private readonly RoamBuilder _builder;

        public RoamBuilderTargetExtension(RoamBuilder builder)
        {
            _builder = builder;
        }

        public IRoamBuilderPageAwareTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel, TTargetPage> With<TTargetPage>() where TTargetPage : Page
            => new RoamBuilderPageAwareTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel, TTargetPage>(_builder);
    }

    internal class RoamBuilderPageAwareTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel, TTargetPage>
        : IRoamBuilderPageAwareTargetExtension<TSourceViewModel, TSourcePage, TTargetViewModel, TTargetPage>
        where TSourceViewModel : BaseViewModel
        where TSourcePage : Page
        where TTargetViewModel : BaseViewModel
        where TTargetPage : Page
    {
        private readonly RoamBuilder _builder;

        public RoamBuilderPageAwareTargetExtension(RoamBuilder builder)
        {
            _builder = builder;
        }

        public IRoamBuilder Do(Func<TSourcePage, TTargetPage, Task> presenterDelegate)
        {
            var existingKey = _builder.Delegates.Keys.FirstOrDefault(key => key.From.PageType == typeof(TSourcePage)
                && key.To.PageType == typeof(TTargetPage));
            if (existingKey != null)
            {
                _builder.Delegates.Remove(existingKey);
            }

            _builder.Delegates.Add(
                new RoamKey<TSourceViewModel, TSourcePage, TTargetViewModel, TTargetPage>(),
                (source, target) =>
                {
                    _builder.Presenter.CurrentRoamState = new RoamState<TTargetViewModel, TTargetPage>();
                    return presenterDelegate(source as TSourcePage, target as TTargetPage);
                });

            return _builder;
        }
    }

    public static class PathwayBuilderRoamExtension
    {
        public static IPathwayBuilder UseRoam(this IPathwayBuilder builder, Action<IRoamBuilder> builderConfig = null)
        {
            var b = builder as PathwayBuilder;

            b.Container.Register<RoamPresenter>();
            var roam = b.Container.Resolve<RoamPresenter>();
            b.Container.RegisterInstance<IPresenter>(roam);
            roam.Init();

            var vmManager = b.Container.Resolve<IViewModelManager>();

            var roamBuilder = new RoamBuilder(roam, vmManager);
            builderConfig(roamBuilder);
            roamBuilder.Build();

            return builder;
        }
    }

    public class AnyViewModel : BaseViewModel
    {
    }

    public class AnyPage : Page
    {
    }

    public class RoamPresenter : BasePresenter
    {
        private readonly IViewModelManager _vmManager;

        public IRoamState CurrentRoamState { get; internal set; }
            = new RoamState<AnyViewModel, AnyPage>();

        public Dictionary<IRoamKey, Func<Page, Page, Task>> Delegates { get; internal set; }
            = new Dictionary<IRoamKey, Func<Page, Page, Task>>();

        public RoamPresenter(IViewModelManager vmManager, INavigator navigator) : base(navigator)
        {
            _vmManager = vmManager;
        }

        public override Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            return Task.CompletedTask;
        }

        public override Task<bool> OnDisplayAlert(string title, string message, string okText, string cancelText = null)
        {
            throw new System.NotImplementedException();
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            await present(page);
        }

        private async Task present(Page page)
        {
            var key = Delegates.Keys.FirstOrDefault(k =>
                        k.From.PageType == CurrentRoamState.PageType
                        && k.To.PageType == page.GetType());

            if (key == null)
            {
                return;
            }

            var presenter = Delegates[key];

            var oldPage = _vmManager.ResolvePageForViewModel(key.From.ViewModelType);

            await presenter.Invoke(oldPage, page);
        }
    }
}
