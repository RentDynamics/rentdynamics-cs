using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources
{
    [PublicAPI]
    public interface IRentDynamicsResourceByClientFactory<[UsedImplicitly] TClient> : IRentDynamicsResourceFactory
        where TClient : IRentDynamicsApiClient
    {
    }
}