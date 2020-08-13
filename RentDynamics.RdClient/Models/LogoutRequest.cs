using Newtonsoft.Json;

namespace RentDynamics.RdClient.Models
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