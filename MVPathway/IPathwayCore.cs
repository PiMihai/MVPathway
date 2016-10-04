using System;
using System.Threading.Tasks;
using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using Xamarin.Forms;

namespace MVPathway
{
  public interface IPathwayCore
  {
    Task<BaseViewModel> CloseViewModelAsync(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null);
    Task<TViewModel> CloseViewModelAsync<TViewModel>(object parameter = null) where TViewModel : BaseViewModel;
    Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null);
    void RegisterInterface<TInterface, TConcrete>(bool asSingleton = true) where TConcrete : TInterface;
    void RegisterPage<TViewModel, TPage>(ViewModelDefinition definition = null)
      where TViewModel : BaseViewModel
      where TPage : class;
    object Resolve(Type type);
    T Resolve<T>();
    Page ResolvePageForViewModel(Func<ViewModelDefinition, bool> definitionFilter);
    Page ResolvePageForViewModel<TViewModel>() where TViewModel : BaseViewModel;
    void SendMessage<TMessage>(TMessage message) where TMessage : IMessage;
    void SetPresenter(BasePresenter presenter);
    Task<BaseViewModel> ShowViewModelAsync(Func<ViewModelDefinition, bool> definitionFilter, object parameter = null);
    Task<TViewModel> ShowViewModelAsync<TViewModel>(object parameter = null) where TViewModel : BaseViewModel;
    void SubscribeToMessage<TMessage>(Action<TMessage> callback) where TMessage : IMessage;
    void UnsubscribeToMessage<TMessage>(Action<TMessage> callback) where TMessage : IMessage;
  }
}