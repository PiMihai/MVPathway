using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static MVPathway.Helpers.MvpHelpers;

namespace MVPathway.Utils.Helpers
{
    public static class NavigationPageExtensions
    {
        public static async Task PushOrPopToPage<TNavigationPage>(this TNavigationPage navigationPage, Page page, bool animated = true)
            where TNavigationPage : NavigationPage
        {
            if (navigationPage.Navigation.NavigationStack.Contains(page))
            {
                while (navigationPage.CurrentPage != page)
                {
                    await navigationPage.PopAsync(animated);
                }
            }

            await OnUiThread(async () =>
                await navigationPage.PushAsync(page, animated));
        }
    }
}
