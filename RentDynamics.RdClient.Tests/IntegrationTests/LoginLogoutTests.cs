using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentDynamics.RdClient.Resources.Authentication;

namespace RentDynamics.RdClient.Tests.IntegrationTests
{
    [TestClass]
    public class LoginLogoutTests : BaseRdApiIntegrationTest
    {
        protected override bool AutomaticAuthentication => false;

        [TestMethod, Ignore("Does not pass with by-pass authentication credentials - 500 error code")]
        public async Task LoginLogoutTest()
        {
            var apiClient = CreateApiClient();
            var authenticationResource = new AuthenticationResource(apiClient);

            await authenticationResource.LoginAsync(RdApiUserName, RdApiPassword);
            await authenticationResource.LogoutAsync();
        }
    }
}