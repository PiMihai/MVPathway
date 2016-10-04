using MVPathway.Integration.Pages;
using MVPathway.Integration.ViewModels;
using MVPathway.Integration.ViewModels.Qualities;
using MVPathway.MVVM.Abstractions;

namespace MVPathway.Integration.Tasks
{
  class PageResolutionTask : BaseIntegrationTask
  {
    public override bool Execute()
    {
      PathwayCore.RegisterPage<FirstViewModel, FirstPage>();
      var definition = new ViewModelDefinition();
      definition.AddQuality<MyQuality>();
      PathwayCore.RegisterPage<SecondViewModel, SecondPage>(definition);

      var firstPage = PathwayCore.ResolvePageForViewModel<FirstViewModel>();
      if(firstPage == null || !(firstPage is FirstPage))
      {
        return false;
      }

      var secondPage = PathwayCore.ResolvePageForViewModel(x => x.HasQuality<MyQuality>());
      if(secondPage == null || !(secondPage is SecondPage))
      {
        return false;
      }
      return true;
    }
  }
}
