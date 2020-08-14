using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RentDynamics.RdClient
{
    public static class RentDynamicsDefaultSettings
    {
        public static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Include,
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        internal const string DateFormatShortUs = "MM\\/dd\\/yyyy";
        internal const string TimeFormatUs = "hh\\:mm tt";
        internal const string CultureNameUs = "en-Us";
    }
}