using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.Authentication
{
    [PublicAPI]
    public class AuthenticationResource : BaseRentDynamicsResource
    {
        private UserAuthentication UserAuthentication => ApiClient.Options.UserAuthentication;

        [UsedImplicitly]
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

            var logoutRequest = new LogoutRequest(UserAuthentication.UserId!.Value);
            await ApiClient.PostAsync<LogoutRequest, object?>("/auth/logout", logoutRequest, cancellationToken);
            UserAuthentication.RemoveAuthentication();
        }
    }
}