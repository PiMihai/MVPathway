using MVPathway.Logging.Abstractions;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using MVPathway.Utils.Messages;
using MVPathway.Utils.ViewModels.Qualities;
using System;
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

        public override async Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter)
        {
            _messagingManager.Send(new MenuToggleMessage());

            if (viewModel.Definition.HasQuality<ChildQuality>() &&
                NavigationStack.Count > 0 &&
                typeof(TViewModel).Name != NavigationStack.Peek().GetType().Name &&
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
                if (_navigationPage == null)
                {
                    _masterDetailPage.Detail = _navigationPage = new NavigationPage(page);
                    _navigationPage.Popped += async (sender, e) => await onNavigationPagePopped(sender, e);
                }
                else
                {
                    var currentPage = _navigationPage.CurrentPage;
                    await _navigationPage.PushAsync(page);
                    _navigationPage.Navigation.RemovePage(currentPage);
                }
                if (viewModel.Definition.HasQuality<ModalQuality>())
                {
                    NavigationPage.SetHasNavigationBar(page, false);
                }
            }
            else if (viewModel.Definition.HasQuality<FullscreenQuality>())
            {
                if (_masterDetailPage == null)
                {
                    Logger.LogError("Cannot show ChildViewModel before MenuViewModel, please show the MenuViewModel first.");
                    return null;
                }
                if (viewModel.Definition.HasQuality<ModalQuality>())
                {
                    NavigationPage.SetHasBackButton(page, false);
                }
                await _navigationPage.PushAsync(page);
            }
            else
            {
                throw new Exception(VIEW_MODEL_NOT_TAGGED_MESSAGE);
            }
            return viewModel;
        }

        public override async Task<TViewModel> Close<TViewModel>(TViewModel viewModel, object parameter)
        {
            if (viewModel.Definition.HasQuality<MenuQuality>())
            {
                Logger.LogError("Cannot close MenuViewModel, it is the root of the app.");
                return null;
            }

            if (await base.Close(viewModel, parameter) == null)
            {
                return null;
            }

            if (viewModel.Definition.HasQuality<FullscreenQuality>())
            {

                NavigationPage.SetHasNavigationBar(_masterDetailPage, true);
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
