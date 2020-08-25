using System;
using Microsoft.Extensions.DependencyInjection;

namespace RentDynamics.RdClient.Resources
{
    public class RentDynamicsResourceByClientFactory<TClient> : IRentDynamicsResourceByClientFactory<TClient>
        where TClient : IRentDynamicsApiClient
    {
        private readonly TClient _apiClient;
        private readonly IServiceProvider _provider;

        public RentDynamicsResourceByClientFactory(TClient apiClient, IServiceProvider provider)
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