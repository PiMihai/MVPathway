using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MVPathway.MVVM
{
    static class PageFactory
    {
        private const string cNoPageRegisteredForViewModelError = "No page registered for this TViewModel type.";
        private const string cCannotGetPageForViewModelError = "Cannot get page for this TViewModel.";

        private static readonly Dictionary<BaseViewModel, Page> mMap = new Dictionary<BaseViewModel, Page>();
        private static readonly Dictionary<Type, Type> mTypeMap = new Dictionary<Type, Type>();

        public static void RegisterPageForViewModel<TViewModel, TPage>()
            where TViewModel : class
            where TPage : class
            => mTypeMap[typeof(TViewModel)] = typeof(TPage);

        public static Page GetPageForViewModel(BaseViewModel viewModel)
        {
            if (mMap.ContainsKey(viewModel))
            {
                return mMap[viewModel];
            }

            var vmType = viewModel.GetType();
            if(!mTypeMap.ContainsKey(vmType))
            {
                throw new Exception(cNoPageRegisteredForViewModelError);
            }

            var page = Activator.CreateInstance(mTypeMap[viewModel.GetType()]) as Page;
            if (page == null)
            {
                throw new Exception(cCannotGetPageForViewModelError);
            }

            page.BindingContext = viewModel;
            mMap[viewModel] = page;
            return page;
        }
    }
}
