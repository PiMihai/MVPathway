using MVPathway.Logging.Abstractions;
using MVPathway.MVVM.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MVPathway.MVVM
{
    class ViewModelManager : IViewModelManager
    {
        private string EXCEPTION_CANNOT_CREATE_PAGE(Type pageType, Type viewModelType) => $"Cannot create {pageType.Name} for {viewModelType.Name}. Make sure the page has a public default constructor.";
        private string EXCEPTION_NO_PAGE_REGISTERED_FOR_VM(Type viewModelType) => $"No page registered for {viewModelType.Name}";

        private readonly IDiContainer _container;
        private readonly ILogger _logger;
        private readonly Dictionary<BaseViewModel, Page> _map = new Dictionary<BaseViewModel, Page>();

        public ViewModelManager(IDiContainer container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        public ViewModelDefinition RegisterPageForViewModel<TViewModel, TPage>()
            where TViewModel : BaseViewModel
            where TPage : class
        {
            _container.Register<TViewModel>();
            var viewModelInstance = _container.Resolve<TViewModel>();
            viewModelInstance.Definition = new ViewModelDefinition();

            var pageInstance = Activator.CreateInstance<TPage>() as Page;
            if (pageInstance == null)
            {
                throw new InvalidOperationException(
                    EXCEPTION_CANNOT_CREATE_PAGE(typeof(TPage), typeof(TViewModel)));
            }

            pageInstance.BindingContext = viewModelInstance;
            _map[viewModelInstance] = pageInstance;

            return viewModelInstance.Definition;
        }

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
    }
}
