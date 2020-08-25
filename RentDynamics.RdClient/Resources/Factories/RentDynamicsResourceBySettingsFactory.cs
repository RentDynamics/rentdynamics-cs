using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient.Resources
{
    [UsedImplicitly, PublicAPI]
    public class RentDynamicsResourceBySettingsFactory<TClientSettings> : IRentDynamicsResourceBySettingsFactory<TClientSettings>
        where TClientSettings : IRentDynamicsApiClientSettings
    {
        private readonly IServiceProvider _serviceProvider;

        public RentDynamicsResourceBySettingsFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TResource CreateResource<TResource>() where TResource : BaseRentDynamicsResource
        {
            var apiClient = _serviceProvider.GetRequiredService<IRentDynamicsApiClient<TClientSettings>>();
            return ActivatorUtilities.CreateInstance<TResource>(_serviceProvider, apiClient);
        }
    }
}