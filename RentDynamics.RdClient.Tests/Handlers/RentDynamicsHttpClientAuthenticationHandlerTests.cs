using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Contrib.HttpClient;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient.Tests.Handlers
{
    [TestClass]
    public class RentDynamicsHttpClientAuthenticationHandlerTests : BaseHandlersTest<RentDynamicsHttpClientAuthenticationHandler>
    {
        private const string TestNonceString = "TestNonceString";
        protected Mock<INonceCalculator> NonceCalculatorMock { get; private set; } = null!;
        protected RentDynamicsOptions Options { get; private set; } = null!;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();

            NonceCalculatorMock = new Mock<INonceCalculator>();
            NonceCalculatorMock.Setup(c => c.GetNonceAsync(It.IsAny<string>(),
                                                      It.IsAny<long>(),
                                                      It.IsAny<string>(),
                                                      It.IsAny<TextReader?>()))
                               .Returns(Task.FromResult(TestNonceString));
            
            Options = new RentDynamicsOptions("testApiKey", "testApiSecret", isDevelopment: true);
        }

        protected override RentDynamicsHttpClientAuthenticationHandler CreateHandlerUnderTest()
        {
            var settings = new RentDynamicsApiClientSettings { Options = Options };
            return new RentDynamicsHttpClientAuthenticationHandler(settings, NonceCalculatorMock.Object);
        }

        [TestMethod]
        public async Task Handler_ShouldAddRdRequiredHeaders_WhenUserIsNotAuthenticated()
        {
            MockHandler.SetupAnyRequest()
                       .ReturnsResponse(HttpStatusCode.OK)
                       .Verifiable();

            await Client.GetAsync("");

            MockHandler.VerifyRequest(HttpMethod.Get, message =>
            {
                message.Headers.GetValues("x-rd-api-key").Should().ContainSingle(Options.ApiKey);
                message.Headers.GetValues("x-rd-timestamp").Should().ContainSingle();
                message.Headers.GetValues("x-rd-api-nonce").Should().ContainSingle(TestNonceString);

                message.Headers.TryGetValues("Authorization", out _).Should().BeFalse();
                return true;
            }, Times.Once());
        }
        
        [TestMethod]
        public async Task Handler_ShouldAddAuthHeader_WhenUserIsAuthenticated()
        {
            const string authToken = "myAuthToken";

            MockHandler.SetupAnyRequest()
                       .ReturnsResponse(HttpStatusCode.OK)
                       .Verifiable();

            
            Options.UserAuthentication.SetAuthentication(100500, authToken);

            await Client.GetAsync("");

            MockHandler.VerifyRequest(HttpMethod.Get, message =>
            {
                message.Headers.GetValues("Authorization").Should().ContainSingle($"TOKEN {authToken}");
                return true;
            }, Times.Once());
        }

        [TestMethod]
        public async Task Handler_ShouldInvokeHandlerCalculator_WhenBodyIsNull()
        {
            const string urlWithQueryParams = "/myResourceUrl?withQuery=1&params=2";

            MockHandler.SetupAnyRequest()
                       .ReturnsResponse(HttpStatusCode.OK);

            await Client.GetAsync(urlWithQueryParams);

            NonceCalculatorMock.Verify(c => c.GetNonceAsync(Options.ApiSecretKey, It.IsAny<long>(), urlWithQueryParams, null));
        }

        [TestMethod]
        public async Task Handler_ShouldInvokeHandlerCalculator_WhenBodyIsNotNull()
        {
            const string urlWithQueryParams = "/myResourceUrl?withQuery=1&params=2";
            const string contentValue = "myContentBody";

            MockHandler.SetupAnyRequest()
                       .ReturnsResponse(HttpStatusCode.OK);
            
            await Client.PostAsync(urlWithQueryParams, new StringContent(contentValue));

            NonceCalculatorMock.Verify(c => c.GetNonceAsync(Options.ApiSecretKey, It.IsAny<long>(), urlWithQueryParams, It.IsNotNull<TextReader?>()));
        }

        [TestMethod]
        public void Test_ConvertNullTypeBodyToNull_EmptyBodyBecomesNull()
        {
            var body = "{}";

            var result = NonceCalculator.ConvertNullTypeBodyToNull(body);

            Assert.AreNotEqual(body, result);
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void Test_ConvertNullTypeBodyToNull_BodyRemainsIntact()
        {
            var body = """{"Test":1}""";

            var result = NonceCalculator.ConvertNullTypeBodyToNull(body);

            Assert.AreEqual(result, body);
        }
    }
}