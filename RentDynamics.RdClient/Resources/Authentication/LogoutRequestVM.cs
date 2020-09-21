using Newtonsoft.Json;

namespace RentDynamics.RdClient.Resources.Authentication
{
    public class LogoutRequestVM
    {
        [JsonProperty("user")]
        public int UserId { get; }

        public LogoutRequestVM(int userId)
        {
            UserId = userId;
        }
    }
}