using System.Threading.Tasks;
using MVPathway.MVVM;

namespace MVPathway.Integration.ViewModels
{
  class FlagViewModel : BaseViewModel
  {
    public bool NavTo { get; private set; }
    public bool NavFrom { get; private set; }

    protected override async Task OnNavigatedTo(object parameter)
    {
      NavTo = true;
    }

    protected override async Task OnNavigatingFrom(object parameter)
    {
      NavFrom = true;
    }
  }
}
