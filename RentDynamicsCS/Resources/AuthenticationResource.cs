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

        public async Task<AuthenticationResponse> LoginAsync(string username, string password, CancellationToken token = default)
        {
            return await _apiClient.PostJsonAsync<AuthenticationRequest, AuthenticationResponse>("/auth/login", new AuthenticationRequest(username, password), token);
        }

        public async Task<AuthenticationResponse> LoginAndSetAuthTokenAsync(string username, string password, RentDynamicsOptions options, CancellationToken token = default)
        {
            var authenticationResponse = await LoginAsync(username, password, token);
            options.AuthToken = authenticationResponse.Token;

            return authenticationResponse;
        }

        public Task LogoutAsync(CancellationToken token = default)
        {
            throw new System.NotImplementedException();
        }
    }

    public static class AuthenticationResourceSyncMethodExtensions
    {
        public static AuthenticationResponse Login(this AuthenticationResource resource, string username, string password)
            => resource.LoginAsync(username, password).GetAwaiter().GetResult();

        public static AuthenticationResponse LoginAndSetAuthTokenAsync(this AuthenticationResource resource, string username, string password, RentDynamicsOptions options)
            => resource.LoginAndSetAuthTokenAsync(username, password, options).GetAwaiter().GetResult();

        public static void Logout(this AuthenticationResource resource)
            => resource.LogoutAsync().GetAwaiter().GetResult();
    }
}