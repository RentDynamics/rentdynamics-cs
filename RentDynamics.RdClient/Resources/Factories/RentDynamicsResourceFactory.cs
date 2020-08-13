using System;
using Microsoft.Extensions.DependencyInjection;

namespace RentDynamics.Client.Resources
{
    public class RentDynamicsResourceFactory<TClient> : IRentDynamicsResourceFactory<TClient>
        where TClient : IRentDynamicsApiClient
    {
        private readonly TClient _apiClient;
        private readonly IServiceProvider _provider;

        public RentDynamicsResourceFactory(TClient apiClient, IServiceProvider provider)
        {
            _apiClient = apiClient;
            _provider = provider;
        }

        public TResource CreateResource<TResource>()
            where TResource : BaseRentDynamicsResource
        {
            return ActivatorUtilities.CreateInstance<TResource>(_provider, _apiClient);
        }
    }
}