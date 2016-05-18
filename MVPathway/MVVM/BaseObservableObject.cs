using System;
using System.ComponentModel;

namespace MVPathway.MVVM
{
    public abstract class BaseObservableObject : INotifyPropertyChanged, IDisposable
    {
        public virtual void Dispose() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
