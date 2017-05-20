namespace MVPathway.Integration.Tasks.Services.Contracts
{
    interface IService
    {
        bool FooCalled { get; }
        void Foo();
    }
}
