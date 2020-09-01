using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RentDynamics.RdClient.HttpApiClient;
using RentDynamics.RdClient.Resources;
using RentDynamics.RdClient.Tests.TestUtils;

namespace RentDynamics.RdClient.Tests.ResourcesDataSerialization
{
    public abstract class BaseResourceDataSerializationTest<TResource>
        where TResource : BaseRentDynamicsResource
    {
        protected Mock<HttpMessageHandler> MockHandler { get; private set; } = null!;
        protected RentDynamicsOptions Options { get; private set; } = null!;

        protected virtual TResource Resource => (TResource) Activator.CreateInstance(typeof(TResource), CreateRdClient())!;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            MockHandler = new Mock<HttpMessageHandler>();
            Options = new RentDynamicsOptions("test", "test-secret", isDevelopment: true);
        }

        protected virtual RentDynamicsApiClient CreateRdClient()
        {
            return CreateRdClient(MockHandler, Options);
        }

        protected static RentDynamicsApiClient CreateRdClient(Mock<HttpMessageHandler> mockHandler, RentDynamicsOptions options)
        {
            var httpClient = mockHandler.CreateClientWithBaseAddress();
            return new RentDynamicsApiClient(httpClient, new RentDynamicsApiClientSettings(options));
        }
    }
}