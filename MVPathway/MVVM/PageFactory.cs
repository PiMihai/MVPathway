using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MVPathway.MVVM
{
    static class PageFactory
    {
        private static readonly Dictionary<BaseViewModel, Page> mMap = new Dictionary<BaseViewModel, Page>();
        private static readonly Dictionary<Type, Type> mTypeMap = new Dictionary<Type, Type>();

        public static void RegisterPageForViewModel<TViewModel, TPage>()
            where TViewModel : class
            where TPage : class
        {
            mTypeMap[typeof(TViewModel)] = typeof(TPage);
        }

        public static Page GetPageForViewModel(BaseViewModel viewModel)
        {
            if (mMap.ContainsKey(viewModel))
            {
                return mMap[viewModel];
            }

            var page = Activator.CreateInstance(mTypeMap[viewModel.GetType()]) as Page;
            if (page == null)
            {
                throw new Exception("Cannot get page for this TViewModel.");
            }
            page.BindingContext = viewModel;
            mMap[viewModel] = page;
            return page;
        }
    }
}
