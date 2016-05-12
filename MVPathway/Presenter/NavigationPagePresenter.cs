using MVPathway.Helpers;
using MVPathway.MVVM;
using MVPathway.Presenter.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Presenter
{
    public class NavigationPagePresenter : BasePresenter
    {
        private const string cInvalidViewModelToCloseError = "TViewModel to close is not the same type as the one on top of the stack.";

        private readonly NavigationPage mNavigationPage;
        private Stack<Type> mViewModelTypeStack = new Stack<Type>();

        public NavigationPagePresenter(NavigationPage navigationPage)
        {
            mNavigationPage = navigationPage;
            Application.Current.MainPage = mNavigationPage;
        }

        protected internal override async Task<BaseViewModel> Show<TViewModel>(object parameter)
        {
            var viewModel = await base.Show<TViewModel>(parameter);
            var page = PageFactory.GetPageForViewModel(viewModel);
            await mNavigationPage.PushAsync(page);
            mViewModelTypeStack.Push(typeof(TViewModel));

            return viewModel;
        }

        protected internal override async Task Close<TViewModel>(object parameter)
        {
            await base.Close<TViewModel>(parameter);
            if(mViewModelTypeStack.Peek() != typeof(TViewModel))
            {
                throw new Exception(cInvalidViewModelToCloseError);
            }
            await mNavigationPage.PopAsync();
            mViewModelTypeStack.Pop();
        }

        protected internal override async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText)
        {
            return await mNavigationPage.DisplayAlert(title, message, okText, cancelText);
        }
    }
}
