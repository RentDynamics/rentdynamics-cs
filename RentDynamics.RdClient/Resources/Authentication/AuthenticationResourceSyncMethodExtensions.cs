using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.Authentication
{
    [PublicAPI]
    public static class AuthenticationResourceSyncMethodExtensions
    {
        public static LoginResponse Login(this AuthenticationResource resource, string username, string password)
            => resource.LoginAsync(username, password).GetAwaiter().GetResult();

        public static void Logout(this AuthenticationResource resource)
            => resource.LogoutAsync().GetAwaiter().GetResult();
    }
}