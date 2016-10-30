using MVPathway.Logging.Abstractions;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Utils.Presenters
{
  public class NavigationPagePresenter : BasePresenter
  {
    private const string cInvalidViewModelToCloseError = "TViewModel to close is not the same type as the one on top of the stack.";

    private readonly ILogger mLogger;
    private readonly NavigationPage mNavigationPage;
    private Stack<Type> mViewModelTypeStack = new Stack<Type>();

    public NavigationPagePresenter(IViewModelManager viewModelManager,
                                   IDiContainer container,
                                   ILogger logger)
      : base(container, viewModelManager)
    {
      mLogger = logger;
      Application.Current.MainPage = mNavigationPage = new NavigationPage();
    }

    public override async Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter)
    {
      await base.Show(viewModel, parameter);

      var page = ViewModelManager.ResolvePageForViewModel(viewModel);
      await mNavigationPage.PushAsync(page);
      mViewModelTypeStack.Push(typeof(TViewModel));

      return viewModel;
    }

    public override async Task<TViewModel> Close<TViewModel>(TViewModel viewModel, object parameter)
    {
      await base.Close(viewModel, parameter);

      if (mViewModelTypeStack.Peek() != typeof(TViewModel))
      {
        mLogger.LogWarning(cInvalidViewModelToCloseError);
        return null;
      }
      await mNavigationPage.PopAsync();
      mViewModelTypeStack.Pop();

      return viewModel;
    }

    public override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
    {
      return await mNavigationPage.DisplayAlert(title, message, okText, cancelText);
    }
  }
}
