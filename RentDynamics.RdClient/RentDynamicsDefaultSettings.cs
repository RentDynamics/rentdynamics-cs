using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using JsonNet.ContractResolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RentDynamics.RdClient
{
    [PublicAPI]
    public static class RentDynamicsDefaultSettings
    {
        public static JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterCamelCasePropertyNamesContractResolver(),
            DefaultValueHandling = DefaultValueHandling.Include,
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Include,
            Formatting = Formatting.None,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            Converters = new List<JsonConverter> { new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal } }
        };

        internal const string DateFormatShortUs = "MM\\/dd\\/yyyy";
        internal const string TimeFormatUs = "hh\\:mm tt";
        internal const string CultureNameUs = "en-Us";
    }
}