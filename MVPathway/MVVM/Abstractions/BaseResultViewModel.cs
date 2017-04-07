using System.Threading.Tasks;

namespace MVPathway.MVVM.Abstractions
{
    public abstract class BaseResultViewModel<T> : BaseViewModel
    {
        private TaskCompletionSource<T> _taskCompletionSource;

        public Task<T> ResultTask => _taskCompletionSource?.Task.IsCompleted ?? true
            ? (_taskCompletionSource = new TaskCompletionSource<T>()).Task
            : _taskCompletionSource.Task;

        protected internal void SetResult(T result)
        {
            _taskCompletionSource?.SetResult(result);
        }
    }
}
