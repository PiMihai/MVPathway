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
    private readonly IDiContainer _container;
    private readonly ILogger _logger;
    private readonly Dictionary<BaseViewModel, Page> _map = new Dictionary<BaseViewModel, Page>();

    public ViewModelManager(IDiContainer container, ILogger logger)
    {
      _container = container;
      _logger = logger;
    }

    public void RegisterPageForViewModel<TViewModel, TPage>(ViewModelDefinition definition = null)
        where TViewModel : BaseViewModel
        where TPage : class
    {
      _container.Register<TViewModel>();
      var viewModelInstance = _container.Resolve<TViewModel>();
      viewModelInstance.Definition = definition ?? new ViewModelDefinition();

      var pageInstance = Activator.CreateInstance<TPage>() as Page;
      if (pageInstance == null)
      {
        _logger.LogError($"Cannot create page for {typeof(TViewModel).Name}");
        _map[viewModelInstance] = null;
        return;
      }

      pageInstance.BindingContext = viewModelInstance;
      _map[viewModelInstance] = pageInstance;
    }

    public BaseViewModel ResolveViewModel(Func<ViewModelDefinition, bool> definitionFilter)
    {
      return _map.Keys.FirstOrDefault(x => definitionFilter(x.Definition));
    }

    public Page ResolvePageForViewModel<TViewModel>(TViewModel viewModel)
      where TViewModel : BaseViewModel
    => getPageForViewModel(viewModel);

    public Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter)
      => getPageForViewModel(ResolveViewModel(definitionFilter));

    private Page getPageForViewModel(BaseViewModel viewModel)
    {
      if (!_map.ContainsKey(viewModel))
      {
        _logger.LogError($"No page registered for {viewModel.GetType().Name}");
        return null;
      }
      return _map[viewModel];
    }
  }
}
