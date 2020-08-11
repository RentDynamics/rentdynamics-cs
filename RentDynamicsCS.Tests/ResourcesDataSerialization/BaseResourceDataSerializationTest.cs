using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RentDynamicsCS.HttpApiClient;
using RentDynamicsCS.Resources;

namespace RentDynamicsCS.Tests.ResourcesDataSerialization
{
    public abstract class BaseResourceDataSerializationTest<TResource>
    where TResource: BaseRentDynamicsResource
    {
        protected Mock<HttpMessageHandler> MockHandler { get; private set; }
        protected virtual TResource Resource => (TResource) Activator.CreateInstance(typeof(TResource), CreateRdClient())!; 

        [TestInitialize]
        public virtual void TestInitialize()
        {
            MockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        }
        
        protected virtual RentDynamicsApiClient CreateRdClient()
        {
            return CreateRdClient(MockHandler);
        }

        protected static RentDynamicsApiClient CreateRdClient(Mock<HttpMessageHandler> mockHandler)
        {
            var options = new RentDynamicsOptions("test", "test-secret", isDevelopment: true);
            var httpClient = mockHandler.CreateClientWithBaseAddress();
            return new RentDynamicsApiClient(httpClient, options);
        }
    }
}