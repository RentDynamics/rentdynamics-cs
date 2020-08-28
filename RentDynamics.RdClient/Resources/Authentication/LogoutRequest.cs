using Newtonsoft.Json;

namespace RentDynamics.RdClient.Resources.Authentication
{
    public class LogoutRequest
    {
        [JsonProperty("user")]
        public int UserId { get; }

        public LogoutRequest(int userId)
        {
            UserId = userId;
        }
    }
}