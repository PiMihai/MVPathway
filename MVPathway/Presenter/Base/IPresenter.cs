using MVPathway.MVVM;
using System;

namespace MVPathway.Presenter
{
    public interface IPresenter
    {
        BaseViewModel Show(Type viewModelType);
        void Close(Type viewModelType);
    }
}
