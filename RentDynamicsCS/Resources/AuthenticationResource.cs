using System.Threading;
using System.Threading.Tasks;
using RentDynamicsCS.Models;

namespace RentDynamicsCS.Resources
{
    public interface IAuthenticationResource
    {
        Task<LoginResponse> LoginAsync(string username, string password, CancellationToken token = default);
        Task<LogoutResponse> LogoutAsync(CancellationToken cancellationToken = default);
    }

    public class AuthenticationResource : BaseRentDynamicsResource, IAuthenticationResource
    {
        public AuthenticationResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public async Task<LoginResponse> LoginAsync(string username, string password, CancellationToken token = default)
        {
            var authenticationResponse = await ApiClient.PostAsync<LoginRequest, LoginResponse>("/auth/login", new LoginRequest(username, password), token);

            ApiClient.Options.AuthToken = authenticationResponse.AuthenticationToken;

            return authenticationResponse;
        }

        public async Task<LogoutResponse> LogoutAsync(CancellationToken cancellationToken = default)
        {
            return await ApiClient.PostAsync<LogoutRequest, LogoutResponse>("/auth/logout", new LogoutRequest(ApiClient.Options.AuthToken), cancellationToken);
        }
    }

    public static class AuthenticationResourceSyncMethodExtensions
    {
        public static LoginResponse Login(this IAuthenticationResource resource, string username, string password)
            => resource.LoginAsync(username, password).GetAwaiter().GetResult();

        public static LogoutResponse Logout(this IAuthenticationResource resource)
            => resource.LogoutAsync().GetAwaiter().GetResult();
    }
}