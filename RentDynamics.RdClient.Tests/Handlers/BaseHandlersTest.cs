using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RentDynamics.RdClient.Tests.TestUtils;

namespace RentDynamics.RdClient.Tests.Handlers
{
    [TestClass]
    public abstract class BaseHandlersTest<THandlerUnderTest> where THandlerUnderTest : DelegatingHandler
    {
        protected Mock<HttpMessageHandler> MockHandler { get; private set; } = null!;

        protected abstract THandlerUnderTest CreateHandlerUnderTest();

        protected HttpClient Client => MockHandler.CreateClient(CreateHandlerUnderTest());

        [TestInitialize]
        public virtual void Initialize()
        {
            MockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        }
    }
}