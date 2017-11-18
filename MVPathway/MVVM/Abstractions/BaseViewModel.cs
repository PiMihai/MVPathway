using System.Threading.Tasks;

namespace MVPathway.MVVM.Abstractions
{
    public abstract class BaseViewModel : BaseObservableObject
    {
        public ViewModelDefinition Definition { get; internal set; }

        protected internal virtual Task OnNavigatedTo(object parameter) => Task.CompletedTask;

        protected internal virtual Task OnNavigatingFrom(object parameter) => Task.CompletedTask;
    }
}
