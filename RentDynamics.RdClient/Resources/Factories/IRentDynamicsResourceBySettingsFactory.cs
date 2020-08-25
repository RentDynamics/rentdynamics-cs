using JetBrains.Annotations;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient.Resources
{
    [PublicAPI]
    public interface IRentDynamicsResourceBySettingsFactory<[UsedImplicitly] TClientSettings> : IRentDynamicsResourceFactory
        where TClientSettings : IRentDynamicsApiClientSettings
    {
    }
}