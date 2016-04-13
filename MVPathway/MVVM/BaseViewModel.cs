using System;
using System.ComponentModel;

namespace MVPathway.MVVM
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        public virtual void OnNavigatedTo(object parameter) { }
        public virtual void OnNavigatingFrom(object parameter) { }
        public virtual void Dispose() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
