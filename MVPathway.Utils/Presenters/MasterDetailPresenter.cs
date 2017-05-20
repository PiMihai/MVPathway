using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM.Abstractions;
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
    public class MasterDetailPresenter<TMasterDetailPage> : BasePresenter
        where TMasterDetailPage : MasterDetailPage
    {
        private const string EXCEPTION_MAIN_CHILD_NOT_REGISTERED = "IMainChildViewModel not registered. When using the MasterDetailPresenter, you must tag one of your ViewModels with the IMainChildQuality quality.";
        private const string EXCEPTION_PARENT_NOT_SHOWN_BEFORE_CHILD = "Cannot show ChildViewModel, please show the ParentViewModel first.";
        private const string EXCEPTION_PARENT_NOT_SHOWN_BEFORE_MODAL = "Cannot show ModalViewModel, please show the ParentViewModel first.";
        private const string EXCEPTION_VM_NOT_TAGGED = "Unknown ViewModel type. When using the MasterDetailPresenter, you must tag all your MVPathway ViewModels with either IParentViewModel, IMainChildViewModel, IChildViewModel or IModalViewModel.";

        private readonly Dictionary<Page, NavigationPage> _cachedChildren = new Dictionary<Page, NavigationPage>();

        private TMasterDetailPage _masterDetailPage;
        private NavigationPage _navigationPage;
        private bool _isHandledPop;

        public bool IsDrawerOpen => _masterDetailPage?.IsPresented ?? false;

        public MasterBehavior MenuBehaviour { get; set; } = MasterBehavior.Popover;

        public MasterDetailPresenter(IViewModelManager vmManager,
                                     IMessagingManager messenger,
                                     IDiContainer container,
                                     ILogger logger)
            : base(container, vmManager, messenger, logger)
        {
        }

        public override async Task Init()
        {
            await base.Init();
            _masterDetailPage = Activator.CreateInstance<TMasterDetailPage>();
            _masterDetailPage.MasterBehavior = MenuBehaviour;
            Messenger.Subscribe<MenuToggleMessage>(onCloseDrawerMessage);
        }

        protected override async Task OnShow(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (viewModel.Definition.HasQuality<IChildQuality>())
            {
                // clean stack of any modal VMs
                while (_navigationPage != null && _navigationPage.Navigation.NavigationStack.Count > 1)
                {
                    await Close();
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
                    var mainChildVm = VmManager.ResolveViewModelByDefinition(def => def.HasQuality<IMainChildQuality>());
                    await Show(mainChildVm, null);
                    Application.Current.MainPage = _masterDetailPage;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(EXCEPTION_MAIN_CHILD_NOT_REGISTERED);
                }
            }
            else if (viewModel.Definition.HasQuality<IChildQuality>())
            {
                if (_masterDetailPage == null)
                {
                    throw new InvalidOperationException(EXCEPTION_PARENT_NOT_SHOWN_BEFORE_CHILD);
                }
                var detailPage = _cachedChildren.ContainsKey(page)
                    ? _cachedChildren[page]
                    : new NavigationPage(page);

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
                    throw new InvalidOperationException(EXCEPTION_PARENT_NOT_SHOWN_BEFORE_MODAL);
                }
                NavigationPage.SetHasNavigationBar(page, !viewModel.Definition.HasQuality<IFullscreenQuality>());
                await _navigationPage.PushAsync(page);
            }
            else
            {
                throw new Exception(EXCEPTION_VM_NOT_TAGGED);
            }
        }

        protected override async Task OnClose(BaseViewModel viewModel, Page page, NavigationRequestType requestType)
        {
            if (viewModel.Definition.HasQuality<IModalQuality>())
            {
                _isHandledPop = true;
                await _navigationPage?.PopAsync();
                _isHandledPop = false;
            }
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
            await Close();
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
