using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace RentDynamics.RdClient.HttpApiClient
{
    [PublicAPI]
    public interface IRentDynamicsApiClientSettings
    {
        RentDynamicsOptions Options { get; set; }
        JsonSerializerSettings JsonSerializerSettings { get; set; }
    }

    public class RentDynamicsApiClientSettings : IRentDynamicsApiClientSettings
    {
        public RentDynamicsOptions Options { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public RentDynamicsApiClientSettings(RentDynamicsOptions options, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            JsonSerializerSettings = jsonSerializerSettings ?? RentDynamicsDefaultSettings.DefaultSerializerSettings;
        }
    }
}