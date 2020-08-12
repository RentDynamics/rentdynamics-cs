using System.Collections.Generic;

namespace RentDynamics.Client.Models
{
    public class ApiError : Dictionary<string, object>
    {
        private string? GetValueOrNull(string key) => TryGetValue("errorMessage", out object result) ? result.ToString() : null;

        public string? ErrorMessage => GetValueOrNull("errorMessage");
        public string? ErrorMessage2 => GetValueOrNull("error_message");
        public string? Detail => GetValueOrNull("detail");
    }
}