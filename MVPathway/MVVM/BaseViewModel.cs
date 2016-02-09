using MVPathway.Helpers;
using System.ComponentModel;
using Xamarin.Forms;

namespace MVPathway.MVVM
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public void Show<TViewModel>() =>
            MessagingCenter.Send(this, Const.CShowViewModel,typeof(TViewModel));

        public void Close<TViewModel>() =>
            MessagingCenter.Send(this, Const.CCloseViewModel,typeof(TViewModel));

        public virtual void OnAppearing() { }
        public virtual void OnDisappearing() { }

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
