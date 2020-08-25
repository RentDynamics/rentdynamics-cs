using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources
{
    [PublicAPI]
    public interface IRentDynamicsResourceFactory
    {
        TResource CreateResource<TResource>() where TResource : BaseRentDynamicsResource;
    }
}