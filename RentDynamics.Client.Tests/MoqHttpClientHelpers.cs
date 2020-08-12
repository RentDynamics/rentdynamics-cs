using System;
using System.Net.Http;
using Moq;
using Moq.Contrib.HttpClient;

namespace RentDynamics.Client.Tests
{
    public static class MoqHttpClientHelpers
    {
        public static HttpClient CreateClientWithBaseAddress(this Mock<HttpMessageHandler> handler, string baseAddress = "http://example.com")
        {
            var client = handler.CreateClient();
            client.BaseAddress = new Uri(baseAddress);
            return client;
        }
    }
}