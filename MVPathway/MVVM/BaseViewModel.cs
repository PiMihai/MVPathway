using System;
using System.ComponentModel;

namespace MVPathway.MVVM
{
    public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
    {
        public virtual void OnAppearing() { }
        public virtual void OnDisappearing() { }
        public virtual void Dispose() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
