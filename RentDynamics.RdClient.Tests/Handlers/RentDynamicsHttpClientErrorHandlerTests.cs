using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Contrib.HttpClient;
using Newtonsoft.Json;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient.Tests.Handlers
{
    [TestClass]
    public class RentDynamicsHttpClientErrorHandlerTests : BaseHandlersTest<RentDynamicsHttpClientErrorHandler>
    {
        protected override RentDynamicsHttpClientErrorHandler CreateHandlerUnderTest()
        {
            var logger = new NullLogger<RentDynamicsHttpClientErrorHandler>();
            
            return new RentDynamicsHttpClientErrorHandler(Mock.Of<RentDynamicsApiClientSettings>(), logger);
        }

        [TestMethod]
        public async Task ErrorHandler_ShouldParseError_OnFailedResponse()
        {
            MockHandler.SetupAnyRequest()
                       .Returns(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
                       {
                           Content = new StringContent(JsonConvert.SerializeObject(new { errorMessage = "My test error message" }))
                       }));

            var exception = await Client.Awaiting(c => c.GetAsync(""))
                                        .Should().ThrowExactlyAsync<RentDynamicsApiException>();

            exception.Which.RawResponseBody.Should().Contain("My test error message");
            exception.Which.ApiError.Should().NotBeNull()
                     .And.ContainKey("errorMessage").WhichValue.Should().Be("My test error message");

            exception.Which.ApiError!.ErrorMessage.Should().Be("My test error message");
        }
        
        [TestMethod]
        public async Task ErrorHandler_ShouldParseError_OnFailedResponse_WhenContentLengthIsMissing()
        {
            MockHandler.SetupAnyRequest()
                       .Returns(() =>
                       {
                           var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                           {
                               Content = new StringContent(JsonConvert.SerializeObject(new { errorMessage = "My test error message" }))
                           };
                           response.Content.Headers.ContentLength = null;
                           return Task.FromResult(response);
                       });

            var exception = await Client.Awaiting(c => c.GetAsync(""))
                                        .Should().ThrowExactlyAsync<RentDynamicsApiException>();

            exception.Which.RawResponseBody.Should().Contain("My test error message");
            exception.Which.ApiError.Should().NotBeNull()
                     .And.ContainKey("errorMessage").WhichValue.Should().Be("My test error message");

            exception.Which.ApiError!.ErrorMessage.Should().Be("My test error message");
        }

        [TestMethod]
        public async Task ErrorHandler_ShouldNotParseError_ForLargeResponse()
        {
            const int responseSizeThreshold = 50 * 1024; //50 KB
            
            string largeContent = string.Join("", Enumerable.Repeat('a', responseSizeThreshold));
            
            MockHandler.SetupAnyRequest()
                       .Returns(() =>
                       {
                           string content = JsonConvert.SerializeObject(new { errorMessage = largeContent });
                           var stringContent = new StringContent(content);
                           var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                           {
                               Content = stringContent
                           };
                           return Task.FromResult(response);
                       });

            var exception = await Client.Awaiting(c => c.GetAsync(""))
                                        .Should().ThrowExactlyAsync<RentDynamicsApiException>();
            
            exception.Which.ApiError.Should().BeNull();
            exception.Which.RawResponseBody.Should().BeNull();
        }

        [TestMethod]
        public async Task ErrorHandler_ShouldNotThrow_WhenResponseContentIsNull()
        {
            MockHandler.SetupAnyRequest()
                       .Returns(() =>
                       {
                           var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                           return Task.FromResult(response);
                       });

            var exception = await Client.Awaiting(c => c.GetAsync(""))
                                        .Should().ThrowExactlyAsync<RentDynamicsApiException>();
            
            exception.Which.ApiError.Should().BeNull();
            exception.Which.RawResponseBody.Should().Be(string.Empty);
        }

        [TestMethod]
        public async Task ErrorHandler_ShouldNotThrow_WhenResponseContent_IsNotSerializableToApiError()
        {
            MockHandler.SetupAnyRequest()
                       .Returns(() =>
                       {
                           var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                           {
                               Content = new StringContent("this string is not serializable to ApiError model")
                           };
                           return Task.FromResult(response);
                       });
            
            var exception = await Client.Awaiting(c => c.GetAsync(""))
                                        .Should().ThrowExactlyAsync<RentDynamicsApiException>();
            
            exception.Which.ApiError.Should().BeNull();
            exception.Which.RawResponseBody.Should().Be("this string is not serializable to ApiError model");
        }
    }
}