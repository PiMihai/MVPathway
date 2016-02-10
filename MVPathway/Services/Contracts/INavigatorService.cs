namespace MVPathway.Services.Contracts
{
    interface INavigatorService
    {
        void Show<TViewModel>();
        void Close<TViewModel>();
    }
}
