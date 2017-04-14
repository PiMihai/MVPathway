namespace MVPathway.MVVM
{
    public class ViewModelResult<TResult>
    {
        public bool Success { get; set; }
        public TResult Result { get; set; }
    }
}
