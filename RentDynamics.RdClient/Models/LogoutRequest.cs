using Newtonsoft.Json;

namespace RentDynamics.RdClient.Models
{
    public class LogoutRequest
    {
        [JsonProperty("authToken")]
        public string AuthenticationToken { get; }

        [JsonProperty("user")]
        public int UserId { get; }

        public LogoutRequest(int userId, string authenticationToken)
        {
            AuthenticationToken = authenticationToken;
            UserId = userId;
        }
    }
}