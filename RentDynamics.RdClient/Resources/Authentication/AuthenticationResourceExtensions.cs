using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.Authentication
{
    [PublicAPI]
    public static class AuthenticationResourceExtensions
    {
        public static LoginResponseVM Login(this AuthenticationResource resource, string username, string password)
            => resource.LoginAsync(username, password).GetAwaiter().GetResult();

        public static void Logout(this AuthenticationResource resource)
            => resource.LogoutAsync().GetAwaiter().GetResult();
    }
}