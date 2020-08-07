using System.Threading;
using System.Threading.Tasks;
using RentDynamicsCS.Models;

namespace RentDynamicsCS.Resources
{
    public class AuthenticationResource
    {
        private readonly IRentDynamicsApiClient _apiClient;

        public AuthenticationResource(IRentDynamicsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<LoginResponse> LoginAsync(string username, string password, CancellationToken token = default)
        {
            var authenticationResponse = await _apiClient.PostAsync<LoginRequest, LoginResponse>("/auth/login", new LoginRequest(username, password), token);

            _apiClient.Options.AuthToken = authenticationResponse.AuthenticationToken;

            return authenticationResponse;
        }

        public async Task<LogoutResponse> LogoutAsync(CancellationToken cancellationToken = default)
        {
            return await _apiClient.PostAsync<LogoutRequest, LogoutResponse>("/auth/logout", new LogoutRequest(_apiClient.Options.AuthToken), cancellationToken);
        }
    }

    public static class AuthenticationResourceSyncMethodExtensions
    {
        public static LoginResponse Login(this AuthenticationResource resource, string username, string password)
            => resource.LoginAsync(username, password).GetAwaiter().GetResult();

        public static LogoutResponse Logout(this AuthenticationResource resource)
            => resource.LogoutAsync().GetAwaiter().GetResult();
    }
}