using MVPathway.MVVM.Abstractions;

namespace MVPathway.MVVM
{
  public abstract class BaseViewModel : BaseObservableObject
  {
    public ViewModelDefinition Definition { get; internal set; }

    protected internal virtual void OnNavigatedTo(object parameter) { }

    protected internal virtual void OnNavigatingFrom(object parameter) { }
  }
}
