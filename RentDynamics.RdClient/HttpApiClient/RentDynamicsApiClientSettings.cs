using Newtonsoft.Json;

namespace RentDynamics.RdClient.HttpApiClient
{
    public class RentDynamicsApiClientSettings
    {
        public RentDynamicsOptions Options { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; } = RentDynamicsDefaultSettings.DefaultSerializerSettings;

    }
}