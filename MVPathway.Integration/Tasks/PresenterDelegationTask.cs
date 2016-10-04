using MVPathway.MVVM.Abstractions;
using System;
using Xamarin.Forms;
using MVPathway.Integration.ViewModels;
using MVPathway.Integration.Presenters;
using MVPathway.Integration.ViewModels.Qualities;

namespace MVPathway.Integration.Tasks
{
  class PresenterDelegationTask : BaseIntegrationTask
  {
    public override bool Execute()
    {
      string cPageException = "Cannot create page";
      PathwayCore.SetPresenter(new MyPresenter(PathwayCore));
      try
      {
        PathwayCore.RegisterPage<FirstViewModel, Page>();
      }
      catch (Exception e) when (e.Message.StartsWith(cPageException)) { }
      try
      {
        var definition = new ViewModelDefinition();
        definition.AddQuality<MyQuality>();
        PathwayCore.RegisterPage<SecondViewModel, Page>(definition);
      }
      catch (Exception e) when (e.Message.StartsWith(cPageException)) { }

      // show
      var first = PathwayCore.ShowViewModelAsync<FirstViewModel>().Result;
      if (first == null || !first.NavTo)
      {
        return false;
      }
      var second = PathwayCore.ShowViewModelAsync(
        x => x.HasQuality<MyQuality>())
        .Result as SecondViewModel;
      if (second == null || !second.NavTo)
      {
        return false;
      }

      // close
      first = PathwayCore.CloseViewModelAsync<FirstViewModel>().Result;
      if (first == null || !first.NavFrom)
      {
        return false;
      }
      second = PathwayCore.CloseViewModelAsync(
        x => x.HasQuality<MyQuality>())
        .Result as SecondViewModel;
      if (second == null || !second.NavFrom)
      {
        return false;
      }

      return PathwayCore.DisplayAlertAsync(null,null,null).Result;
    }
  }
}
