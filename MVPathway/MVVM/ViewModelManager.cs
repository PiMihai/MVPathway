using MVPathway.Logging.Abstractions;
using MVPathway.MVVM.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace MVPathway.MVVM
{
    public class ViewModelManager : IViewModelManager
    {
        private static string EXCEPTION_CANNOT_CREATE_PAGE(Type pageType, Type viewModelType) => $"Cannot create {pageType.Name} for {viewModelType.Name}. Make sure the page has a public default constructor.";
        private static string EXCEPTION_NO_PAGE_REGISTERED_FOR_VM(Type viewModelType) => $"No page registered for {viewModelType.Name}";

        private readonly IDiContainer _container;
        private readonly ILogger _logger;
        private readonly Dictionary<BaseViewModel, Page> _map = new Dictionary<BaseViewModel, Page>();

        public ViewModelManager(IDiContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        public void AutoScanAndRegister(Assembly assembly)
        {
            var viewModelTypes = assembly
                .DefinedTypes
                .Where(t => typeof(BaseViewModel).GetTypeInfo().IsAssignableFrom(t))
                .ToList();
            var pageTypes = assembly
                .DefinedTypes
                .Where(t => typeof(Page).GetTypeInfo().IsAssignableFrom(t))
                .ToList();

            foreach (var viewModelType in viewModelTypes)
            {
                var name = viewModelType.Name.Replace("ViewModel", "");
                var pageType = pageTypes.FirstOrDefault(t => t.Name.Replace("Page", "") == name);
                if (pageType == null)
                {
                    continue;
                }
                registerPageForViewModel(viewModelType.AsType(), pageType.AsType());
            }
        }

        public ViewModelDefinition RegisterPageForViewModel<TViewModel, TPage>()
            where TViewModel : BaseViewModel
            where TPage : class
            => registerPageForViewModel(typeof(TViewModel), typeof(TPage));

        public BaseViewModel ResolveViewModelByDefinition(Func<ViewModelDefinition, bool> definitionFilter)
        {
            return _map.Keys.FirstOrDefault(x => definitionFilter(x.Definition));
        }

        public List<BaseViewModel> ResolveViewModelsByDefinition(Func<ViewModelDefinition, bool> definitionFilter)
        {
            return _map.Keys.Where(x => definitionFilter(x.Definition)).ToList();
        }

        public Page ResolvePageForViewModel<TViewModel>()
          where TViewModel : BaseViewModel
        {
            var viewModel = _container.Resolve<TViewModel>();
            return ResolvePageForViewModel(viewModel);
        }

        public Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter)
        {
            var viewModel = ResolveViewModelByDefinition(definitionFilter);
            return ResolvePageForViewModel(viewModel);
        }

        public Page ResolvePageForViewModel(BaseViewModel viewModel)
        {
            if (!_map.ContainsKey(viewModel))
            {
                throw new InvalidOperationException(
                    EXCEPTION_NO_PAGE_REGISTERED_FOR_VM(viewModel.GetType()));
            }
            return _map[viewModel];
        }

        public List<Page> ResolvePagesForViewModels(Func<ViewModelDefinition, bool> definitionFilter)
        {
            return ResolveViewModelsByDefinition(definitionFilter)
                .Select(ResolvePageForViewModel)
                .ToList();
        }

        public ViewModelDefinition ResolveDefinitionForViewModel<TViewModel>()
            where TViewModel : BaseViewModel
            => ResolveDefinitionForViewModel(_container.Resolve<TViewModel>());

        public ViewModelDefinition ResolveDefinitionForViewModel(BaseViewModel viewModel)
            => viewModel.Definition;

        public List<Page> ResolvePagesForViewModels(List<BaseViewModel> viewModels)
        {
            return viewModels.Select(ResolvePageForViewModel).ToList();
        }

        private ViewModelDefinition registerPageForViewModel(Type viewModelType, Type pageType)
        {
            _container.Register(viewModelType);
            var viewModelInstance = (BaseViewModel)_container.Resolve(viewModelType);
            viewModelInstance.Definition = new ViewModelDefinition();

            var pageInstance = Activator.CreateInstance(pageType) as Page;
            if (pageInstance == null)
            {
                throw new InvalidOperationException(
                    EXCEPTION_CANNOT_CREATE_PAGE(pageType, viewModelType));
            }

            pageInstance.BindingContext = viewModelInstance;
            _map[viewModelInstance] = pageInstance;

            return viewModelInstance.Definition;
        }
    }
}
