using System.Threading.Tasks;

namespace MVPathway.MVVM.Abstractions
{
  public abstract class BaseResultViewModel<T> : BaseViewModel
  {
    internal TaskCompletionSource<T> TaskCompletionSource { get; set; }

    protected internal void SetResult(T result)
    {
      TaskCompletionSource?.SetResult(result);
    }
  }
}
