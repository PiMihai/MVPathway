using MVPathway.Integration.Services.Contracts;
using System;

namespace MVPathway.Integration.Services
{
    public class CacheService : ICacheService
    {
        private Type _presenterType;
        public Type PresenterType
        {
            get => _presenterType;
            set => _presenterType = value;
        }
    }
}
