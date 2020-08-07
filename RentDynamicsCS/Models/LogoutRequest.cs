using Newtonsoft.Json;

namespace RentDynamicsCS.Models
{
    public class LogoutRequest
    {
        [JsonProperty("authToken")]
        public string AuthenticationToken { get; set; }

        public LogoutRequest(string authenticationToken)
        {
            AuthenticationToken = authenticationToken;
        }
    }
}