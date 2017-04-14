using System.Threading.Tasks;

namespace MVPathway.MVVM.Abstractions
{
    public abstract class BaseResultViewModel<TResult> : BaseViewModel
    {
        private TaskCompletionSource<ViewModelResult<TResult>> _taskCompletionSource;

        public Task<ViewModelResult<TResult>> ResultTask => (_taskCompletionSource?.Task?.IsCompleted ?? true)
            ? (_taskCompletionSource = new TaskCompletionSource<ViewModelResult<TResult>>()).Task
            : _taskCompletionSource.Task;

        protected internal void SetResult(TResult result)
        {
            _taskCompletionSource?.SetResult(new ViewModelResult<TResult>
            {
                Success = true,
                Result = result
            });
        }

        protected internal void Cancel()
        {
            _taskCompletionSource?.SetResult(new ViewModelResult<TResult>
            {
                Success = false,
                Result = default(TResult)
            });
        }
    }
}
