using Newtonsoft.Json;

namespace RentDynamicsCS.HttpApiClient
{
    public class RentDynamicsApiClientSettings
    {
        public RentDynamicsOptions Options { get; set; }
        public JsonSerializerSettings? JsonSerializerSettings { get; set; }
    }
}