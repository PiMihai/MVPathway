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
        }

        protected override async Task<BaseViewModel> Show(Type viewModelType)
        {
            var viewModel = await base.Show(viewModelType);
            var page = PageFactory.GetPageForViewModel(viewModel);
            await mNavigationPage.PushAsync(page);
            mViewModelTypeStack.Push(viewModelType);

            return viewModel;
        }

        protected override async Task Close(Type viewModelType)
        {
            await base.Close(viewModelType);
            if(mViewModelTypeStack.Peek() != viewModelType)
            {
                throw new Exception(cInvalidViewModelToCloseError);
            }
            await mNavigationPage.PopAsync();
            mViewModelTypeStack.Pop();
        }
    }
}
