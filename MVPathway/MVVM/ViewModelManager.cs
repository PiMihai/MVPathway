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

        private readonly Dictionary<Type, ViewModelDefinition> _definitionMap
            = new Dictionary<Type, ViewModelDefinition>();
        private readonly Dictionary<BaseViewModel, Page> _pageCache
            = new Dictionary<BaseViewModel, Page>();
        private readonly Dictionary<Type, Func<Page>> _pageFactories
            = new Dictionary<Type, Func<Page>>();

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
                RegisterPageForViewModel(viewModelType.AsType(), pageType.AsType());
            }
        }

        public ViewModelDefinition RegisterPageForViewModel<TViewModel, TPage>()
            where TViewModel : BaseViewModel
            where TPage : class
            => RegisterPageForViewModel(typeof(TViewModel), typeof(TPage));

        public ViewModelDefinition RegisterPageForViewModel(Type viewModelType, Type pageType)
        {
            _container.Register(viewModelType);

            if (_definitionMap.ContainsKey(viewModelType))
            {
                _definitionMap.Remove(viewModelType);
            }
            _definitionMap[viewModelType] = new ViewModelDefinition();

            _pageFactories[viewModelType] = () =>
            {
                var viewModelInstance = (BaseViewModel)_container.Resolve(viewModelType);
                viewModelInstance.Definition = _definitionMap[viewModelType];

                var pageInstance = Activator.CreateInstance(pageType) as Page;
                if (pageInstance == null)
                {
                    throw new InvalidOperationException(
                        EXCEPTION_CANNOT_CREATE_PAGE(pageType, viewModelType));
                }

                pageInstance.BindingContext = viewModelInstance;
                _pageCache[viewModelInstance] = pageInstance;
                return pageInstance;
            };

            return _definitionMap[viewModelType];
        }

        public BaseViewModel ResolveViewModelByDefinition(Func<ViewModelDefinition, bool> definitionFilter)
        {
            return ResolveViewModelsByDefinition(definitionFilter).FirstOrDefault();
        }

        public IEnumerable<BaseViewModel> ResolveViewModelsByDefinition(Func<ViewModelDefinition, bool> definitionFilter)
        {
            return _definitionMap.Where(x => definitionFilter(x.Value))
                .Select(x => _container.Resolve(x.Key))
                .Cast<BaseViewModel>();
        }

        public Page ResolvePageForViewModel<TViewModel>()
          where TViewModel : BaseViewModel
        {
            return ResolvePageForViewModel(typeof(TViewModel));
        }

        public Page ResolvePageForViewModel(Type viewModelType)
        {
            var viewModel = (BaseViewModel)_container.Resolve(viewModelType);
            return ResolvePageForViewModel(viewModel);
        }

        public Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter)
        {
            var viewModel = ResolveViewModelByDefinition(definitionFilter);
            return ResolvePageForViewModel(viewModel);
        }

        public Page ResolvePageForViewModel(BaseViewModel viewModel)
        {
            var viewModelType = viewModel.GetType();
            if (!_pageFactories.ContainsKey(viewModelType))
            {
                throw new InvalidOperationException(
                    EXCEPTION_NO_PAGE_REGISTERED_FOR_VM(viewModelType));
            }
            if (!_pageCache.Keys.Any(k => k.GetType() == viewModelType))
            {
                var page = _pageFactories[viewModelType].Invoke();
                _pageCache[viewModel] = page;
            }
            return _pageCache[viewModel];
        }

        public IEnumerable<Page> ResolvePagesForViewModels(Func<ViewModelDefinition, bool> definitionFilter)
        {
            return ResolveViewModelsByDefinition(definitionFilter)
                .Select(ResolvePageForViewModel);
        }

        public List<Page> ResolvePagesForViewModels(List<BaseViewModel> viewModels)
        {
            return viewModels.Select(ResolvePageForViewModel).ToList();
        }

        public ViewModelDefinition ResolveDefinitionForViewModel<TViewModel>()
            where TViewModel : BaseViewModel
        {
            return _definitionMap[typeof(TViewModel)];
        }
    }
}
