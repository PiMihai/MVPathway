using System.Threading.Tasks;
using MVPathway.MVVM.Abstractions;

namespace MVPathway.MVVM
{
  public abstract class BaseViewModel : BaseObservableObject
  {
    public ViewModelDefinition Definition { get; internal set; }

    protected internal virtual async Task OnNavigatedTo(object parameter) { }

    protected internal virtual async Task OnNavigatingFrom(object parameter) { }
  }
}
