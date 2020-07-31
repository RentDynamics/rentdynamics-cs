using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace RentDynamicsCS.RestClient
{
    public static class RentDynamicsRestClientConfigurationHelper
    {
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new OrderedContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Include,
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        public static void ConfigureClient(IRestClient client, RentDynamicsOptions options, JsonSerializerSettings jsonSerializerSettings)
        {
            client.Authenticator = new RentDynamicsRequestAuthenticator(options);
            client.ThrowOnAnyError = true;
            client.UseNewtonsoftJson(jsonSerializerSettings);
        }

        public static void ConfigureClient(IRestClient client, RentDynamicsOptions options)
            => ConfigureClient(client, options, DefaultSerializerSettings);
    }
}