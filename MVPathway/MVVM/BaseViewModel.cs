namespace MVPathway.MVVM
{
    public abstract class BaseViewModel : BaseObservableObject
    {
        public virtual void OnNavigatedTo(object parameter) { }
        public virtual void OnNavigatingFrom(object parameter) { }
    }
}
