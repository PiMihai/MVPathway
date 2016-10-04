using MVPathway.MVVM;

namespace MVPathway.Integration.ViewModels
{
  class FlagViewModel : BaseViewModel
  {
    public bool NavTo { get; private set; }
    public bool NavFrom { get; private set; }

    protected override void OnNavigatedTo(object parameter)
    {
      NavTo = true;
    }

    protected override void OnNavigatingFrom(object parameter)
    {
      NavFrom = true;
    }
  }
}
