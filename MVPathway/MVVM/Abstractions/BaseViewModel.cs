using System.Threading.Tasks;

namespace MVPathway.MVVM.Abstractions
{
  public abstract class BaseViewModel : BaseObservableObject
  {
    public ViewModelDefinition Definition { get; internal set; }

    protected internal virtual async Task OnNavigatedTo(object parameter) { }

    protected internal virtual async Task OnNavigatingFrom(object parameter) { }
  }
}
