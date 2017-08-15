using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Navigation;
using MVPathway.Navigation.Abstractions;
using MVPathway.Presenters;
using MVPathway.Utils.Messages;
using MVPathway.Utils.ViewModels.Qualities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;

namespace MVPathway.Utils.Presenters
{
    public class MasterDetailPresenter : MasterDetailPresenter<MasterDetailPage, NavigationPage>
    {
        public MasterDetailPresenter(IMessenger messenger, IViewModelManager vmManager, INavigator navigator) : base(messenger, vmManager, navigator)
        {
        }
    }
    public class MasterDetailPresenter<TMasterDetailPage> : MasterDetailPresenter<TMasterDetailPage, NavigationPage>
        where TMasterDetailPage : MasterDetailPage
    {
        public MasterDetailPresenter(IMessenger messenger, IViewModelManager vmManager, INavigator navigator) : base(messenger, vmManager, navigator)
        {
        }
    }

    public class MasterDetailPresenter<TMasterDetailPage, TNavigationPage> : BasePresenter
        where TMasterDetailPage : MasterDetailPage
        where TNavigationPage : NavigationPage
    {
        private const string _EXCEPTION_MAIN_CHILD_NOT_REGISTERED = "IMainChildViewModel not registered. When using the MasterDetailPresenter, you must tag one of your ViewModels with the IMainChildQuality quality.";
        private const string _EXCEPTION_PARENT_NOT_SHOWN_BEFORE_CHILD = "Cannot show ChildViewModel, please show the ParentViewModel first.";
        private const string _EXCEPTION_PARENT_NOT_SHOWN_BEFORE_MODAL = "Cannot show ModalViewModel, please show the ParentViewModel first.";
        private const string _EXCEPTION_VM_NOT_TAGGED = "Unknown ViewModel type. When using the MasterDetailPresenter, you must tag all your MVPathway ViewModels with either IParentViewModel, IMainChildViewModel, IChildViewModel or IModalViewModel.";

        private readonly IMessenger _messenger;
        private readonly IViewModelManager _vmManager;
        private readonly Dictionary<Page, NavigationPage> _cachedChildren = new Dictionary<Page, NavigationPage>();

        private TMasterDetailPage _masterDetailPage;
        private TNavigationPage _navigationPage;
        private bool _isHandledPop;

        public bool IsDrawerOpen => _masterDetailPage?.IsPresented ?? false;

        public MasterBehavior MenuBehaviour { get; set; } = MasterBehavior.Popover;

        public MasterDetailPresenter(IMessenger messenger,
                                     IViewModelManager vmManager,
                                     INavigator navigator)
            : base(navigator)
        {
            _messenger = messenger;
            _vmManager = vmManager;
        }

        public override async Task Init()
        {
            await base.Init();
            _masterDetailPage = Activator.CreateInstance<TMasterDetailPage>();
            _masterDetailPage.MasterBehavior = MenuBehaviour;
            _messenger.Subscribe<MenuToggleMessage>(onCloseDrawerMessage);
        }

        public override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (viewModel.Definition.HasQuality<IChildQuality>())
            {
                // clean stack of any modal VMs
                while (_navigationPage != null && _navigationPage.Navigation.NavigationStack.Count > 1)
                {
                    await Navigator.Close(this);
                }
            }

            if (viewModel.Definition.HasQuality<IParentQuality>()) // AICI AICI AICI
            {
                if (string.IsNullOrEmpty(page.Title))
                {
                    page.Title = "Menu";
                }
                _masterDetailPage.Master = page;
                try
                {
                    var mainChildVm = _vmManager.ResolveViewModelByDefinition(def => def.HasQuality<IMainChildQuality>());
                    await Navigator.Show(mainChildVm);
                    await OnUiThread(() => Application.Current.MainPage = _masterDetailPage);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(_EXCEPTION_MAIN_CHILD_NOT_REGISTERED);
                }
            }
            else if (viewModel.Definition.HasQuality<IChildQuality>())
            {
                if (_masterDetailPage == null)
                {
                    throw new InvalidOperationException(_EXCEPTION_PARENT_NOT_SHOWN_BEFORE_CHILD);
                }
                var detailPage = (TNavigationPage)(_cachedChildren.ContainsKey(page)
                    ? _cachedChildren[page]
                    : Activator.CreateInstance(typeof(TNavigationPage), page));

                NavigationPage.SetHasNavigationBar(page, !viewModel.Definition.HasQuality<IFullscreenQuality>());

                if (_navigationPage == null)
                {
                    _masterDetailPage.Detail = _navigationPage = detailPage;
                    _navigationPage.Popped += async (sender, e) => await onNavigationPagePopped(sender, e);
                }
                else
                {
                    await OnUiThread(async () =>
                    {
                        var currentPage = _navigationPage.CurrentPage;
                        await _navigationPage.PushAsync(page);
                        if (page != currentPage)
                        {
                            _navigationPage.Navigation.RemovePage(currentPage);
                        }
                        Application.Current.MainPage = _masterDetailPage;
                    });
                }
            }
            else if (viewModel.Definition.HasQuality<IModalQuality>())
            {
                if (_navigationPage == null)
                {
                    throw new InvalidOperationException(_EXCEPTION_PARENT_NOT_SHOWN_BEFORE_MODAL);
                }
                NavigationPage.SetHasNavigationBar(page, !viewModel.Definition.HasQuality<IFullscreenQuality>());
                await _navigationPage.PushAsync(page);
            }
            else
            {
                throw new Exception(_EXCEPTION_VM_NOT_TAGGED);
            }
        }

        public override async Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (viewModel.Definition.HasQuality<IModalQuality>())
            {
                _isHandledPop = true;
                if (_navigationPage != null)
                {
                    await _navigationPage?.PopAsync();
                }
                _isHandledPop = false;
            }
        }

        public override async Task<bool> OnDisplayAlert(string title, string message, string okText, string cancelText = null)
        {
            if (cancelText != null)
            {
                return await Application.Current.MainPage?.DisplayAlert(title, message, okText, cancelText);
            }
            await Application.Current.MainPage?.DisplayAlert(title, message, okText);
            return true;
        }

        private async Task onNavigationPagePopped(object sender, NavigationEventArgs e)
        {
            if (_isHandledPop)
            {
                return;
            }
            var viewModel = e.Page.BindingContext as BaseViewModel;
            if (viewModel == null)
            {
                return;
            }
            await Navigator.Close(this);
        }

        private void onCloseDrawerMessage(MenuToggleMessage message)
        {
            if (_masterDetailPage == null)
            {
                return;
            }
            _masterDetailPage.IsPresented = !_masterDetailPage.IsPresented;
        }
    }
}
