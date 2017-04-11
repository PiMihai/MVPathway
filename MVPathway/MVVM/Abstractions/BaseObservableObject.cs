using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MVPathway.MVVM.Abstractions
{
    public abstract class BaseObservableObject : INotifyPropertyChanged, IDisposable
    {
        public virtual void Dispose() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
