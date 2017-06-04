using System;
using System.Threading.Tasks;

namespace MVPathway.Integration.Services.Contracts
{
    public interface IViewModelDefiner
    {
        void DefineInitial();
        Task RedefineBasedOnPresenterType(Type presenterType);
    }
}
