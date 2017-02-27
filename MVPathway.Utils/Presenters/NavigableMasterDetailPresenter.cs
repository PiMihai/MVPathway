using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using MVPathway.Utils.Messages;
using MVPathway.Utils.ViewModels.Qualities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVPathway.Logging.Abstractions;
using MVPathway.MVVM;
using Xamarin.Forms;

namespace MVPathway.Utils.Presenters
{
  public class NavigableMasterDetailPresenter : BasePresenter
  {
    private const string cViewModelNotTaggedMessage = "Unknown ViewModel type. If you are using the NavigableMasterDetailPresenters, you must tag all your MVPathway ViewModels with either IMenuViewModel, IMainChildViewModel, IChildViewModel or IFullViewModel.";
    private const string cMainChildNotRegisteredMessage = "IMainChildViewModel not registered. Please register your main child view model as an interface before trying to show the menu.";
    private const string cInvalidViewModelToCloseError = "TViewModel to close is not the same type as the one on top of the stack.";

    private readonly ILogger mLogger;
    private readonly Stack<BaseViewModel> mViewModelStack = new Stack<BaseViewModel>();
    private MasterDetailPage mMasterDetailPage;

    public bool IsDrawerOpen => mMasterDetailPage?.IsPresented ?? false;

    public MasterBehavior MenuBehaviour { get; set; }

    public NavigableMasterDetailPresenter(IViewModelManager viewModelManager,
                                          IMessagingManager messagingManager,
                                          ILogger logger,
                                          IDiContainer container)
        : base(container, viewModelManager)
    {
      mLogger = logger;
      MenuBehaviour = MasterBehavior.Popover;
      messagingManager.Subscribe<MenuToggleMessage>(onCloseDrawerMessage);
    }

    public override async Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter)
    {
      await base.Show(viewModel, parameter);

      var page = ViewModelManager.ResolvePageForViewModel(viewModel);
      if (viewModel.Definition.HasQuality<MenuQuality>())
      {
        if (mMasterDetailPage == null)
        {
          if (string.IsNullOrEmpty(page.Title))
          {
            page.Title = "Menu";
          }
          mMasterDetailPage = new MasterDetailPage
          {
            MasterBehavior = MenuBehaviour,
            Master = page,
          };
        }
        try
        {
          var mainChildVm = ViewModelManager.ResolveViewModel(def => def.HasQuality<MainChildQuality>());
          await Show(mainChildVm, null);
        }
        catch
        {
          throw new Exception(cMainChildNotRegisteredMessage);
        }
        Application.Current.MainPage = mMasterDetailPage;
      }
      else if (viewModel.Definition.HasQuality<ChildQuality>())
      {
        if (mMasterDetailPage == null)
        {
          var menuVm = ViewModelManager.ResolveViewModel(def => def.HasQuality<MenuQuality>());
          await Show(menuVm, null);
        }
        if (mMasterDetailPage != null)
        {
          mMasterDetailPage.Detail = new NavigationPage(page);
        }
      }
      else if (viewModel.Definition.HasQuality<FullscreenQuality>())
      {
        NavigationPage.SetHasNavigationBar(page, true);
        Application.Current.MainPage = new NavigationPage(page);
      }
      else
      {
        throw new Exception(cViewModelNotTaggedMessage);
      }

      mViewModelStack.Push(viewModel);
      return viewModel;
    }

    public override async Task<TViewModel> Close<TViewModel>(TViewModel viewModel, object parameter)
    {
      await base.Close(viewModel, parameter);

      if (mViewModelStack.Peek() != viewModel)
      {
        mLogger.LogWarning(cInvalidViewModelToCloseError);
        return null;
      }

      mViewModelStack.Pop();
      if (viewModel.Definition.HasQuality<ChildQuality>())
      {
        var mainChildVm = ViewModelManager.ResolveViewModel(def => def.HasQuality<MainChildQuality>());
        await Show(mainChildVm, null);
      }
      else if (viewModel.Definition.HasQuality<FullscreenQuality>())
      {
        var lastVm = mViewModelStack.Peek();
        await Show(lastVm, null);
      }
      
      return viewModel;
    }

    public override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
    {
      if (cancelText != null)
      {
        return await Application.Current.MainPage?.DisplayAlert(title, message, okText, cancelText);
      }
      await Application.Current.MainPage?.DisplayAlert(title, message, okText);
      return true;
    }

    private void onCloseDrawerMessage(MenuToggleMessage message)
    {
      if (mMasterDetailPage == null)
      {
        return;
      }
      mMasterDetailPage.IsPresented = !mMasterDetailPage.IsPresented;
    }
  }
}
