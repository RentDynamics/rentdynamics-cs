using Newtonsoft.Json;

namespace RentDynamicsCS.Models
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        
        [JsonProperty("token")]
        public string AuthenticationToken { get; set; }
    }
}