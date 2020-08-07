using System.Collections.Generic;

namespace RentDynamicsCS.Models
{
    public class ApiError
    {
        public string? ErrorMessage { get; set; }
        public string? Detail { get; set; }

        public IEnumerable<string> GetErrors()
        {
            if (ErrorMessage != null) yield return ErrorMessage;
            if (Detail != null) yield return Detail;
        }
    }
}