using System.Threading.Tasks;
using MVPathway.Integration.Pages;
using MVPathway.Integration.Tasks.Base;
using MVPathway.Integration.ViewModels;
using MVPathway.Integration.ViewModels.Qualities;
using MVPathway.MVVM.Abstractions;

namespace MVPathway.Integration.Tasks.Core
{
  public class PageResolutionTask : CoreIntegrationTask
  {
    private readonly IDiContainer mContainer;
    private readonly IViewModelManager mViewModelmanager;

    public PageResolutionTask(IDiContainer container, IViewModelManager viewModelManager)
    {
      mContainer = container;
      mViewModelmanager = viewModelManager;
    }

    public override async Task<bool> Execute()
    {
      await base.Execute();

      mViewModelmanager.RegisterPageForViewModel<FirstViewModel, FirstPage>();
      var definition = new ViewModelDefinition();
      definition.AddQuality<MyQuality>();
      mViewModelmanager.RegisterPageForViewModel<SecondViewModel, SecondPage>(definition);

      var firstPage = mViewModelmanager.ResolvePageForViewModel(mContainer.Resolve<FirstViewModel>());
      if(!(firstPage is FirstPage))
      {
        return false;
      }

      var secondPage = mViewModelmanager.ResolvePageForViewModel(x => x.HasQuality<MyQuality>());
      return secondPage is SecondPage;
    }
  }
}
