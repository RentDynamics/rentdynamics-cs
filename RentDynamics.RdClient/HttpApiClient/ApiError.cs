using System.Collections.Generic;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.HttpApiClient
{
    [PublicAPI]
    public class ApiError : Dictionary<string, object>
    {
        private string? GetValueOrNull(string key) => TryGetValue(key, out object result) ? result.ToString() : null;

        public string? ErrorMessage => GetValueOrNull("errorMessage");
        public string? ErrorMessage2 => GetValueOrNull("error_message");
        public string? Detail => GetValueOrNull("detail");
    }
}