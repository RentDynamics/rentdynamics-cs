using Newtonsoft.Json;

namespace RentDynamics.RdClient.Resources.Authentication
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        
        [JsonProperty("token")]
        public string AuthenticationToken { get; set; }
    }
}