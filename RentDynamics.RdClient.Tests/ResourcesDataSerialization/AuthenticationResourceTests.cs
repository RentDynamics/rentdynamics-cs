using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.Contrib.HttpClient;
using Newtonsoft.Json.Linq;
using RentDynamics.RdClient.Resources.Authentication;

namespace RentDynamics.RdClient.Tests.ResourcesDataSerialization
{
    [TestClass]
    public class AuthenticationResourceTests : BaseResourceDataSerializationTest<AuthenticationResource>
    {
        [TestMethod]
        public async Task LoginRequest_ShouldBeSuccessful()
        {
            const string username = "user";
            const string password = "password";
            using var sha = new SHA1Managed();
            string hashedPassword = sha.ComputeHash(Encoding.UTF8.GetBytes(password)).ToHexString().ToLower();

            MockHandler.SetupAnyRequest()
                       .ReturnsResponse(HttpStatusCode.OK, "{\"userId\":100500, \"token\":\"myResponseToken\"}", MediaTypeNames.Application.Json);


            var loginResponse = await Resource.LoginAsync(username, password);

            MockHandler.VerifyRequest(async message =>
            {
                message.RequestUri.PathAndQuery.Should().Be("/auth/login");

                var jContent = await message.Content.ReadAsAsync<JObject>();
                jContent.Property("username").Value.ToString().Should().Be(username);
                jContent.Property("password").Value.ToString().Should().Be(hashedPassword);
                return true;
            });

            loginResponse.AuthenticationToken.Should().Be("myResponseToken");
            loginResponse.UserId.Should().Be(100500);

            Options.UserAuthentication.IsAuthenticated.Should().BeTrue();
            Options.UserAuthentication.UserId.Should().Be(100500);
            Options.UserAuthentication.AuthenticationToken.Should().Be("myResponseToken");
        }

        [TestMethod]
        public async Task LogoutRequest_ShouldBeSuccessful()
        {
            Options.UserAuthentication.SetAuthentication(100500, "myToken");

            MockHandler.SetupAnyRequest()
                       .ReturnsResponse(HttpStatusCode.NoContent, Stream.Null);

            await Resource.LogoutAsync();

            MockHandler.VerifyRequest(message =>
            {
                message.RequestUri.PathAndQuery.Should().Be("/auth/logout");

                return true;
            });

            Options.UserAuthentication.IsAuthenticated.Should().BeFalse();
        }

        [TestMethod]
        public async Task LogoutRequest_ShouldBeFailed_WhenUserIsNotAuthenticated()
        {
            Options.UserAuthentication.RemoveAuthentication();

            await Resource.Awaiting(r => r.LogoutAsync())
                          .Should()
                          .ThrowExactlyAsync<AuthenticationResourceException>()
                          .WithMessage("User is not authenticated");
        }
    }
}