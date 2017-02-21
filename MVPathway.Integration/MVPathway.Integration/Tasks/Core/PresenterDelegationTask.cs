using MVPathway.MVVM.Abstractions;
using System;
using System.Threading.Tasks;
using MVPathway.Integration.ViewModels;
using MVPathway.Integration.ViewModels.Qualities;
using MVPathway.Integration.Tasks.Base;
using MVPathway.Utils.ViewModels.Qualities;
using MVPathway.Integration.Pages;
using MVPathway.Presenters.Abstractions;

namespace MVPathway.Integration.Tasks.Core
{
  public class PresenterDelegationTask : CoreIntegrationTask
  {
    private readonly IDiContainer mContainer;
    private readonly IViewModelManager mViewModelManager;
    private IPresenter mPresenter;

    public PresenterDelegationTask(IDiContainer container, IViewModelManager viewModelManager)
    {
      mContainer = container;
      mViewModelManager = viewModelManager;
    }

    public override async Task<bool> Execute()
    {
      mPresenter = mContainer.Resolve<IPresenter>();
      const string cPageException = "Cannot create page";

      var menuVmDef = new ViewModelDefinition();
      menuVmDef.AddQuality<MenuQuality>();
      menuVmDef.AddQuality<MyQuality>();
      var mainChildVmDef = new ViewModelDefinition();
      mainChildVmDef.AddQuality<MainChildQuality>();

      try
      {
        mViewModelManager.RegisterPageForViewModel<FirstViewModel, FirstPage>(mainChildVmDef);
      }
      catch (Exception e) when (e.Message.StartsWith(cPageException)) { }
      try
      {
        mViewModelManager.RegisterPageForViewModel<SecondViewModel, SecondPage>(menuVmDef);
      }
      catch (Exception e) when (e.Message.StartsWith(cPageException)) { }

      // show
      var first = mPresenter.Show<FirstViewModel>().Result;
      if (first == null || !first.NavTo)
      {
        return false;
      }
      var second = await mPresenter.Show(
        def => def.HasQuality<MyQuality>()) as SecondViewModel;
      if (second == null || !second.NavTo)
      {
        return false;
      }

      // close
      second = await mPresenter.Close(
        x => x.HasQuality<MyQuality>()) as SecondViewModel;
      if (second == null || !second.NavFrom)
      {
        return false;
      }
      first = await mPresenter.Close<FirstViewModel>();

      return first != null && first.NavFrom;

      // TODO : find way to test alerts
      //return PathwayCore.DisplayAlertAsync("Alert", "Are you sure?", "Ok").Result;
    }
  }
}
