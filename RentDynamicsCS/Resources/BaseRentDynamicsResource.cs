namespace RentDynamicsCS.Resources
{
    public abstract class BaseRentDynamicsResource
    {
        protected IRentDynamicsApiClient ApiClient { get; }

        protected BaseRentDynamicsResource(IRentDynamicsApiClient apiClient)
        {
            ApiClient = apiClient;
        }
    }
}