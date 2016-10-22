using MVPathway.Integration.Services.Contracts;

namespace MVPathway.Integration.Services
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
