using MVPathway.Messages.Abstractions;
using MVPathway.MVVM;
using MVPathway.MVVM.Abstractions;
using MVPathway.Presenters;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MVPathway
{
  class PathwayCore : IPathwayCore
  {
    #region Instance members

    private BasePresenter mPresenter;
    private readonly IViewModelManager mViewModelManager;
    private readonly IDiContainer mContainer;
    private readonly IMessagingManager mMessagingManager;

    #endregion

    #region Constructors

    public PathwayCore(IDiContainer container,
                         IViewModelManager viewModelManager,
                         IMessagingManager messagingManager)
    {
      mContainer = container;
      mViewModelManager = viewModelManager;
      mMessagingManager = messagingManager;
    }

    #endregion

    #region Public methods

    public void SetPresenter(BasePresenter presenter)
    {
      mPresenter = presenter;
    }

    public async Task<TViewModel> ShowViewModelAsync<TViewModel>(object parameter = null)
      where TViewModel : BaseViewModel
      => await mPresenter.Show<TViewModel>(parameter);

    public async Task<BaseViewModel> ShowViewModelAsync(Func<ViewModelDefinition, bool> definitionFilter,
                                                object parameter = null)
      => await mPresenter.Show(mViewModelManager.ResolveViewModel(definitionFilter), parameter);

    public async Task<TViewModel> CloseViewModelAsync<TViewModel>(object parameter = null)
      where TViewModel : BaseViewModel
      => await mPresenter.Close<TViewModel>(parameter);

    public async Task<BaseViewModel> CloseViewModelAsync(Func<ViewModelDefinition, bool> definitionFilter,
                                                 object parameter = null)
      => await mPresenter.Close(mViewModelManager.ResolveViewModel(definitionFilter), parameter);

    public async Task<bool> DisplayAlertAsync(string title, string message, string okText, string cancelText = null)
      => await mPresenter.DisplayAlertAsync(title, message, okText, cancelText);

    public void RegisterInterface<TInterface, TConcrete>(bool asSingleton = true)
      where TConcrete : TInterface
      => mContainer.Register<TInterface, TConcrete>(asSingleton);

    public T Resolve<T>()
      => mContainer.Resolve<T>();

    public object Resolve(Type type)
      => mContainer.Resolve(type);

    public void RegisterPage<TViewModel, TPage>(ViewModelDefinition definition = null)
    where TViewModel : BaseViewModel
      where TPage : class
      => mViewModelManager.RegisterPageForViewModel<TViewModel, TPage>(definition);

    public Page ResolvePageForViewModel<TViewModel>()
      where TViewModel : BaseViewModel
      => mViewModelManager.ResolvePageForViewModel<TViewModel>();

    public Page ResolvePageForViewModel(Func<ViewModelDefinition,bool> definitionFilter)
      => mViewModelManager.ResolvePageForViewModel(definitionFilter);

    public void SubscribeToMessage<TMessage>(Action<TMessage> callback)
      where TMessage : IMessage
      => mMessagingManager.Subscribe(callback);

    public void UnsubscribeToMessage<TMessage>(Action<TMessage> callback)
      where TMessage : IMessage
      => mMessagingManager.Unsubscribe(callback);

    public void SendMessage<TMessage>(TMessage message)
      where TMessage : IMessage
      => mMessagingManager.Send(message);

    #endregion

    #region Internal methods
    
    #endregion
  }
}