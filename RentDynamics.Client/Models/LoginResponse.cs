using Newtonsoft.Json;

namespace RentDynamics.Client.Models
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        
        [JsonProperty("token")]
        public string AuthenticationToken { get; set; }
    }
}