namespace MVPathway.MVVM
{
    public abstract class BaseViewModel : BaseObservableObject
    {
        protected internal virtual void OnNavigatedTo(object parameter) { }
        protected internal virtual void OnNavigatingFrom(object parameter) { }
    }
}
