using System.Threading;
using System.Threading.Tasks;
using RentDynamicsCS.Models;

namespace RentDynamicsCS.Resources
{
    public class AuthenticationResource
    {
        private readonly RentDynamicsApiClient _apiClient;

        public AuthenticationResource(RentDynamicsApiClient apiClient)
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
}