using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenters
{
  public class NavigationPagePresenter : BasePresenter
  {
    private const string cInvalidViewModelToCloseError = "TViewModel to close is not the same type as the one on top of the stack.";

    private readonly NavigationPage mNavigationPage;
    private Stack<Type> mViewModelTypeStack = new Stack<Type>();

    public NavigationPagePresenter(IPathwayCore pathwayCore, NavigationPage navigationPage)
      : base(pathwayCore)
    {
      mNavigationPage = navigationPage;
      Application.Current.MainPage = mNavigationPage;
    }

    protected override async Task<TViewModel> Show<TViewModel>(TViewModel viewModel, object parameter)
    {
      var page = PathwayCore.ResolvePageForViewModel<TViewModel>();
      await mNavigationPage.PushAsync(page);
      mViewModelTypeStack.Push(typeof(TViewModel));

      return viewModel;
    }

    protected override async Task<TViewModel> Close<TViewModel>(TViewModel viewModel, object parameter)
    {
      if (mViewModelTypeStack.Peek() != typeof(TViewModel))
      {
        throw new Exception(cInvalidViewModelToCloseError);
      }
      await mNavigationPage.PopAsync();
      mViewModelTypeStack.Pop();

      return viewModel;
    }

    protected override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText)
    {
      return await mNavigationPage.DisplayAlert(title, message, okText, cancelText);
    }
  }
}
