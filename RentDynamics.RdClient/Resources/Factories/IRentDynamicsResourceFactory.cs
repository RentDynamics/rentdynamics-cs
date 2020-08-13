namespace RentDynamics.RdClient.Resources
{
    public interface IRentDynamicsResourceFactory<TClient> : IRentDynamicsResourceFactory
        where TClient : IRentDynamicsApiClient
    {
    }

    public interface IRentDynamicsResourceFactory
    {
        TResource CreateResource<TResource>() where TResource : BaseRentDynamicsResource;
    }
}