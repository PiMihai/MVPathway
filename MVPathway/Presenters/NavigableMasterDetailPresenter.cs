using MVPathway.Messages;
using MVPathway.Messages.Messengers;
using MVPathway.MVVM;
using MVPathway.Presenters.Abstractions;
using MVPathway.Presenters.Base;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenters
{
  public class NavigableMasterDetailPresenter : BasePresenter
  {
    private const string cViewModelNotTaggedMessage = "Unknown ViewModel type. If you are using the NavigableMasterDetailPresenters, you must tag all your MVPathway ViewModels with either IMenuViewModel, IMainChildViewModel, IChildViewModel or IFullViewModel.";
    private const string cMainChildNotRegisteredMessage = "IMainChildViewModel not registered. Please register your main child view model as an interface before trying to show the menu.";

    private MasterDetailPage mMasterDetailPage;
    
    public MasterBehavior MenuBehaviour { get; set; }

    public NavigableMasterDetailPresenter()
        : base()
    {
      MenuBehaviour = MasterBehavior.Popover;

      var messenger = MessengerResolver.RegisterMessenger<MenuToggleMessenger, MenuToggleMessage>();
      MessagingCenter.Subscribe<MenuToggleMessenger, MenuToggleMessage>(messenger, MenuToggleMessenger.CMessageKey, onCloseDrawerMessage);
    }

    protected internal override async Task<BaseViewModel> Show<TViewModel>(object parameter)
    {
      var viewModel = await base.Show<TViewModel>(parameter);
      var page = PathwayCore.GetPageForViewModel(viewModel);

      if (viewModel is IMenuViewModel)
      {
        if (mMasterDetailPage == null)
        {
          if(string.IsNullOrEmpty(page.Title))
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
          await PathwayCore.ShowViewModelAsync<IMainChildViewModel>();
        }
        catch
        {
          throw new Exception(cMainChildNotRegisteredMessage);
        }
        Application.Current.MainPage = mMasterDetailPage;
      }
      else if (viewModel is IChildViewModel)
      {
        mMasterDetailPage.Detail = new NavigationPage(page);
      }
      else if (viewModel is IFullViewModel)
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

    protected internal override async Task Close<TViewModel>(object parameter)
    {
      await base.Close<TViewModel>(parameter);
      var viewModel = PathwayCore.Resolve<TViewModel>();
      if (viewModel is IFullViewModel)
      {
        await PathwayCore.ShowViewModelAsync<IMenuViewModel>();
      }
    }

    protected internal override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText)
    {
      if (cancelText != null)
      {
        return await Application.Current.MainPage.DisplayAlert(title, message, okText, cancelText);
      }
      await Application.Current.MainPage.DisplayAlert(title, message, okText);
      return true;
    }

    private void onCloseDrawerMessage(MenuToggleMessenger sender, MenuToggleMessage message)
    {
      if (mMasterDetailPage == null)
      {
        return;
      }
      mMasterDetailPage.IsPresented = !mMasterDetailPage.IsPresented;
    }
  }
}
