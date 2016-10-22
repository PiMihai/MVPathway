﻿using MVPathway.MVVM.Abstractions;
using System;
using MVPathway.Integration.ViewModels;
using MVPathway.Integration.Presenters;
using MVPathway.Integration.ViewModels.Qualities;
using MVPathway.Integration.Tasks.Base;
using MVPathway.Utils.ViewModels.Qualities;
using MVPathway.Integration.Pages;
using MVPathway.Presenters.Abstractions;

namespace MVPathway.Integration.Tasks.Core
{
  public class PresenterDelegationTask : CoreIntegrationTask
  {
    private readonly IPresenter mPresenter;
    private readonly IViewModelManager mViewModelManager;

    public PresenterDelegationTask(IPresenter presenter, IViewModelManager viewModelManager)
    {
      mPresenter = presenter;
      mViewModelManager = viewModelManager;
    }

    public override bool Execute()
    {
      string cPageException = "Cannot create page";

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
      var second = mPresenter.Show(
        def => def.HasQuality<MyQuality>())
        .Result as SecondViewModel;
      if (second == null || !second.NavTo)
      {
        return false;
      }

      // close
      first = mPresenter.Close<FirstViewModel>().Result;
      if (first == null || !first.NavFrom)
      {
        return false;
      }
      second = mPresenter.Close(
        x => x.HasQuality<MyQuality>())
        .Result as SecondViewModel;
      if (second == null || !second.NavFrom)
      {
        return false;
      }

      // TODO : find way to test alerts
      //return PathwayCore.DisplayAlertAsync("Alert", "Are you sure?", "Ok").Result;

      return true;
    }
  }
}