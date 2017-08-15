using System;
using System.Threading.Tasks;

namespace MVPathway.Integration.Services.Contracts
{
    public interface IViewModelDefiner
    {
        void Init();
        Task RedefineBasedOnPresenterType(Type presenterType);
    }
}
