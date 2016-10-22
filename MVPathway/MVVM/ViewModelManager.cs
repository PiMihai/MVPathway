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
    private readonly IDiContainer mContainer;
    private readonly ILogger mLogger;
    private readonly Dictionary<BaseViewModel, Page> mMap = new Dictionary<BaseViewModel, Page>();

    public ViewModelManager(IDiContainer container, ILogger logger)
    {
      mContainer = container;
      mLogger = logger;
    }

    public void RegisterPageForViewModel<TViewModel, TPage>(ViewModelDefinition definition = null)
        where TViewModel : BaseViewModel
        where TPage : class
    {
      mContainer.Register<TViewModel>();
      var viewModelInstance = mContainer.Resolve<TViewModel>();
      viewModelInstance.Definition = definition ?? new ViewModelDefinition();

      var pageInstance = Activator.CreateInstance<TPage>() as Page;
      if (pageInstance == null)
      {
        mLogger.LogError($"Cannot create page for {typeof(TViewModel).Name}");
        mMap[viewModelInstance] = null;
        return;
      }

      pageInstance.BindingContext = viewModelInstance;
      mMap[viewModelInstance] = pageInstance;
    }

    public BaseViewModel ResolveViewModel(Func<ViewModelDefinition, bool> definitionFilter)
    {
      return mMap.Keys.FirstOrDefault(x => definitionFilter(x.Definition));
    }

    public Page ResolvePageForViewModel<TViewModel>(TViewModel viewModel)
      where TViewModel : BaseViewModel
    => getPageForViewModel(viewModel);

    public Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter)
      => getPageForViewModel(ResolveViewModel(definitionFilter));

    private Page getPageForViewModel(BaseViewModel viewModel)
    {
      if (!mMap.ContainsKey(viewModel))
      {
        mLogger.LogError($"No page registered for {viewModel.GetType().Name}");
        return null;
      }
      return mMap[viewModel];
    }
  }
}
