using Newtonsoft.Json;

namespace RentDynamics.RdClient.HttpApiClient
{
    public interface IRentDynamicsApiClientSettings
    {
        RentDynamicsOptions Options { get; set; }
        JsonSerializerSettings? JsonSerializerSettings { get; set; }
    }

    public class RentDynamicsApiClientSettings : IRentDynamicsApiClientSettings
    {
        public RentDynamicsOptions Options { get; set; }
        public JsonSerializerSettings? JsonSerializerSettings { get; set; }
    }
}