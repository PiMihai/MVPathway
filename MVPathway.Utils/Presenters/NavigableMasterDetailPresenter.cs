using MVPathway.Presenters;
using MVPathway.Utils.Messages;
using MVPathway.Utils.ViewModels.Qualities;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Utils.Presenters
{
  public class NavigableMasterDetailPresenter : BasePresenter
  {
    private const string cViewModelNotTaggedMessage = "Unknown ViewModel type. If you are using the NavigableMasterDetailPresenters, you must tag all your MVPathway ViewModels with either IMenuViewModel, IMainChildViewModel, IChildViewModel or IFullViewModel.";
    private const string cMainChildNotRegisteredMessage = "IMainChildViewModel not registered. Please register your main child view model as an interface before trying to show the menu.";

    private MasterDetailPage mMasterDetailPage;

    public MasterBehavior MenuBehaviour { get; set; }

    public NavigableMasterDetailPresenter(IPathwayCore pathwayCore)
        : base(pathwayCore)
    {
      MenuBehaviour = MasterBehavior.Popover;
      PathwayCore.SubscribeToMessage<MenuToggleMessage>(onCloseDrawerMessage);
    }

    protected override async Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter)
    {
      var page = PathwayCore.ResolvePageForViewModel<TViewModel>();
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
          await PathwayCore.ShowViewModelAsync(x => x.HasQuality<MainChildQuality>());
        }
        catch
        {
          throw new Exception(cMainChildNotRegisteredMessage);
        }
        Application.Current.MainPage = mMasterDetailPage;
      }
      else if (viewModel.Definition.HasQuality<ChildQuality>())
      {
        mMasterDetailPage.Detail = new NavigationPage(page);
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
      return viewModel;
    }

    protected override async Task<TViewModel> Close<TViewModel>(TViewModel viewModel, object parameter)
    {
      if (viewModel.Definition.HasQuality<ChildQuality>())
      {
        await PathwayCore.ShowViewModelAsync(x => x.HasQuality<MainChildQuality>());
      }
      return viewModel;
    }
    
    protected override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText)
    {
      if (cancelText != null)
      {
        return await Application.Current.MainPage.DisplayAlert(title, message, okText, cancelText);
      }
      await Application.Current.MainPage.DisplayAlert(title, message, okText);
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
