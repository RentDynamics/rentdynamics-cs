using System;
using System.Net.Http;
using Moq;
using Moq.Contrib.HttpClient;

namespace RentDynamics.RdClient.Tests.TestUtils
{
    public static class MoqHttpClientHelpers
    {
        private static HttpClient SetBaseAddress(this HttpClient client, string baseAddress)
        {
            client.BaseAddress = new Uri(baseAddress);
            return client;
        }

        public static HttpClient CreateClientWithBaseAddress(this Mock<HttpMessageHandler> handler, string baseAddress = "http://example.com")
        {
            var client = handler.CreateClient();
            return client.SetBaseAddress(baseAddress);
        }

        public static HttpClient CreateClient(this Mock<HttpMessageHandler> messageHandler, DelegatingHandler outerHandler, string baseAddress = "http://example.com")
        {
            outerHandler.InnerHandler = messageHandler.Object;
            var httpClient = new HttpClient(outerHandler);
            
            return httpClient.SetBaseAddress(baseAddress);
        }
    }
}