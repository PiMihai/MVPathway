namespace MVPathway.Services.Contracts
{
    interface INavigatorService
    {
        void Show<TViewModel>(object parameter = null);
        void Close<TViewModel>(object parameter = null);
    }
}
