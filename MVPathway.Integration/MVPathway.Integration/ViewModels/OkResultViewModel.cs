using System.Threading.Tasks;
using MVPathway.MVVM.Abstractions;

namespace MVPathway.Integration.ViewModels
{
  class OkResultViewModel : BaseResultViewModel<string>
  {
    protected override async Task OnNavigatedTo(object parameter)
    {
      await Task.Delay(1234);
      SetResult("Ok");
    }
  }
}
