using MVPathway.MVVM;
using MVPathway.Presenter.Base;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MVPathway.Presenter
{
    public class NavigationPagePresenter : BasePresenter
    {
        private readonly NavigationPage mNavigationPage;
        private Stack<Type> mViewModelTypeStack = new Stack<Type>();

        public NavigationPagePresenter(NavigationPage navigationPage)
        {
            mNavigationPage = navigationPage;
        }

        public override BaseViewModel Show(Type viewModelType)
        {
            var viewModel = base.Show(viewModelType);
            var page = PageFactory.GetPageForViewModel(viewModel);
            mNavigationPage.PushAsync(page);
            mViewModelTypeStack.Push(viewModelType);

            return viewModel;
        }

        public override void Close(Type viewModelType)
        {
            base.Close(viewModelType);
            if(mViewModelTypeStack.Peek() != viewModelType)
            {
                throw new Exception("TViewModel to close is not the same type as the one on top of the stack.");
            }
            mNavigationPage.PopAsync();
            mViewModelTypeStack.Pop();
        }
    }
}
