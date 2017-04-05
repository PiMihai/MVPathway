using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using MVPathway.Utils.Messages;
using MVPathway.Utils.ViewModels.Qualities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Utils.Presenters
{
    public class NavigableMasterDetailPresenter<TMasterDetailPage> : BasePresenter
        where TMasterDetailPage : MasterDetailPage
    {
        private const string VIEW_MODEL_NOT_TAGGED_MESSAGE = "Unknown ViewModel type. If you are using the NavigableMasterDetailPresenters, you must tag all your MVPathway ViewModels with either IMenuViewModel, IMainChildViewModel, IChildViewModel or IFullViewModel.";
        private const string MAIN_CHILD_NOT_REGISTERED_MESSAGE = "IMainChildViewModel not registered. Please register your main child view model as an interface before trying to show the menu.";

        private readonly IMessagingManager _messagingManager;

        private readonly Dictionary<Page, NavigationPage> _cachedChildren = new Dictionary<Page, NavigationPage>();

        private TMasterDetailPage _masterDetailPage;
        private NavigationPage _navigationPage;
        private bool _isHandledPop;

        public bool IsDrawerOpen => _masterDetailPage?.IsPresented ?? false;

        public MasterBehavior MenuBehaviour { get; set; }

        public NavigableMasterDetailPresenter(IViewModelManager viewModelManager,
                                              IMessagingManager messagingManager,
                                              IDiContainer container,
                                              ILogger logger)
            : base(container, viewModelManager, logger)
        {
            _messagingManager = messagingManager;

            MenuBehaviour = MasterBehavior.Popover;
            _messagingManager.Subscribe<MenuToggleMessage>(onCloseDrawerMessage);
        }

        public override async Task<BaseViewModel> Show(BaseViewModel viewModel, object parameter)
        {
            if (viewModel.Definition.HasQuality<ChildQuality>() &&
              NavigationStack.Count > 0 &&
              NavigationStack.Peek().GetType().Name != viewModel.GetType().Name &&
              NavigationStack.Peek().Definition.HasQuality<ChildQuality>())
            {
                await Close(NavigationStack.Peek(), null);
            }

            if (await base.Show(viewModel, parameter) == null)
            {
                return null;
            }

            var page = ViewModelManager.ResolvePageForViewModel(viewModel);
            if (viewModel.Definition.HasQuality<MenuQuality>())
            {
                if (_masterDetailPage == null)
                {
                    if (string.IsNullOrEmpty(page.Title))
                    {
                        page.Title = "Menu";
                    }
                    _masterDetailPage = Activator.CreateInstance<TMasterDetailPage>();
                    _masterDetailPage.MasterBehavior = MenuBehaviour;
                    _masterDetailPage.Master = page;
                }
                try
                {
                    var mainChildVm = ViewModelManager.ResolveViewModel(def => def.HasQuality<MainChildQuality>());
                    await Show(mainChildVm, null);
                    Application.Current.MainPage = _masterDetailPage;
                }
                catch
                {
                    throw new Exception(MAIN_CHILD_NOT_REGISTERED_MESSAGE);
                }
            }
            else if (viewModel.Definition.HasQuality<ChildQuality>())
            {
                if (_masterDetailPage == null)
                {
                    Logger.LogError("Cannot show ChildViewModel before MenuViewModel, please show the MenuViewModel first.");
                    return null;
                }
                var detailPage = _cachedChildren.ContainsKey(page)
                    ? _cachedChildren[page]
                    : new NavigationPage(page);

                NavigationPage.SetHasNavigationBar(page, !viewModel.Definition.HasQuality<ModalQuality>());
                NavigationPage.SetHasNavigationBar(detailPage, !viewModel.Definition.HasQuality<ModalQuality>());

                if (_navigationPage == null)
                {
                    _masterDetailPage.Detail = _navigationPage = detailPage;
                    _navigationPage.Popped += async (sender, e) => await onNavigationPagePopped(sender, e);
                }
                else
                {
                    var currentPage = _navigationPage.CurrentPage;
                    await _navigationPage.PushAsync(page);
                    _navigationPage.Navigation.RemovePage(currentPage);
                }
            }
            else if (viewModel.Definition.HasQuality<FullscreenQuality>())
            {
                if (_masterDetailPage == null)
                {
                    Logger.LogError("Cannot show ChildViewModel before MenuViewModel, please show the MenuViewModel first.");
                    return null;
                }
                NavigationPage.SetHasNavigationBar(page, !viewModel.Definition.HasQuality<ModalQuality>());
            }
            else
            {
                throw new Exception(VIEW_MODEL_NOT_TAGGED_MESSAGE);
            }
            return viewModel;
        }

        public override async Task<BaseViewModel> Close(BaseViewModel viewModel, object parameter)
        {
            if (viewModel.Definition.HasQuality<MenuQuality>())
            {
                Logger.LogError("Cannot close MenuViewModel, it is the root of the app.");
                return null;
            }

            if (viewModel.Definition.HasQuality<ChildQuality>())
            {
                await Show(def => def.HasQuality<MainChildQuality>());
                return viewModel;
            }

            if (await base.Close(viewModel, parameter) == null)
            {
                return null;
            }

            if (viewModel.Definition.HasQuality<FullscreenQuality>())
            {
                _isHandledPop = true;
                await _navigationPage.PopAsync();
                _isHandledPop = false;
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
            await base.Close(viewModel, null);
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
