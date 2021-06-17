using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.HttpApiClient
{
    [PublicAPI]
    public class ApiError : Dictionary<string, object>
    {
        private string? GetValueOrNull(string key) => TryGetValue(key, out object result) ? result.ToString() : null;

        public string? ErrorMessage
        {
            get => GetValueOrNull("errorMessage");
            set => this["errorMessage"] = value!;
        }

        public string? ErrorMessage2 => GetValueOrNull("error_message");
        public string? Detail => GetValueOrNull("detail");

        public string CombineAllErrorMessages()
        {
            var errorMessages = new[] { ErrorMessage, ErrorMessage2, Detail }.Where(x => !string.IsNullOrWhiteSpace(x));
            return string.Join(",", errorMessages);
        }
    }
}