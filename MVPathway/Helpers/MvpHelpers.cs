using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway.Helpers
{
    public static class MvpHelpers
    {
        public static async Task OnUiThread(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            await tcs.Task;
        }
    }
}
