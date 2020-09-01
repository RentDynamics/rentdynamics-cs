using JetBrains.Annotations;
using Newtonsoft.Json;

namespace RentDynamics.RdClient.Resources.Authentication
{
    [PublicAPI]
    public class LoginResponse
    {
        public int UserId { get; }

        [JsonProperty("token")]
        public string AuthenticationToken { get; }

        [UsedImplicitly]
        public LoginResponse(int userId, string authenticationToken)
        {
            UserId = userId;
            AuthenticationToken = authenticationToken;
        }
    }
}