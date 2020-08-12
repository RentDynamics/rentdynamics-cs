using System.Threading;
using System.Threading.Tasks;
using RentDynamics.Client.Models;

namespace RentDynamics.Client.Resources
{
    public class AuthenticationResource : BaseRentDynamicsResource
    {
        private UserAuthentication UserAuthentication => ApiClient.Options.UserAuthentication;

        public AuthenticationResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public async Task<LoginResponse> LoginAsync(string username, string password, CancellationToken token = default)
        {
            var authenticationResponse = await ApiClient.PostAsync<LoginRequest, LoginResponse>("/auth/login", new LoginRequest(username, password), token);

            UserAuthentication.SetAuthentication(authenticationResponse.UserId, authenticationResponse.AuthenticationToken);
            return authenticationResponse;
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (!UserAuthentication.IsAuthenticated) throw new AuthenticationResourceException("User is not authenticated");

            var logoutRequest = new LogoutRequest(UserAuthentication.UserId.Value, UserAuthentication.AuthenticationToken);
            await ApiClient.PostAsync<LogoutRequest, object?>("/auth/logout", logoutRequest, cancellationToken);
            UserAuthentication.RemoveAuthentication();
        }
    }
}