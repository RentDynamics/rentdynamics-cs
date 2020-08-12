using RentDynamicsCS.Models;

namespace RentDynamicsCS.Resources
{
    public static class AuthenticationResourceSyncMethodExtensions
    {
        public static LoginResponse Login(this IAuthenticationResource resource, string username, string password)
            => resource.LoginAsync(username, password).GetAwaiter().GetResult();

        public static void Logout(this IAuthenticationResource resource)
            => resource.LogoutAsync().GetAwaiter().GetResult();
    }
}