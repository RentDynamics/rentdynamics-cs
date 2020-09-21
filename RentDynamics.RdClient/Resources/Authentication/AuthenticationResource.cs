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

        public async Task<LoginResponseVM> LoginAsync(string username, string password, CancellationToken token = default)
        {
            var authenticationResponse = await ApiClient.PostAsync<LoginRequestVM, LoginResponseVM>("/auth/login", new LoginRequestVM(username, password), token);

            UserAuthentication.SetAuthentication(authenticationResponse.UserId, authenticationResponse.AuthenticationToken);
            return authenticationResponse;
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            if (!UserAuthentication.IsAuthenticated) throw new AuthenticationResourceException("User is not authenticated");

            var logoutRequest = new LogoutRequestVM(UserAuthentication.UserId!.Value);
            await ApiClient.PostAsync<LogoutRequestVM, object?>("/auth/logout", logoutRequest, cancellationToken);
            UserAuthentication.RemoveAuthentication();
        }
    }
}