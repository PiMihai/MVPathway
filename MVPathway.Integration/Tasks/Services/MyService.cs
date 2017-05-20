using MVPathway.Integration.Tasks.Services.Contracts;

namespace MVPathway.Integration.Tasks.Services
{
    class MyService : IService
    {
        public bool FooCalled { get; private set; }

        public void Foo()
        {
            FooCalled = true;
        }
    }
}
