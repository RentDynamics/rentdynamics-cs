using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace RentDynamicsCS.HttpApiClient
{
    public static class RentDynamicsHttpClientFactory
    {
        public static HttpClient Create(RentDynamicsOptions options, JsonSerializerSettings? jsonSerializerSettings)
        {
            var httpClientHandler = new HttpClientHandler();

            var errorHandler = new RentDynamicsHttpClientErrorHandler(jsonSerializerSettings ?? RentDynamicsDefaultSettings.DefaultSerializerSettings)
            {
                InnerHandler = httpClientHandler
            };

            var authenticator = new RentDynamicsHttpClientAuthenticationHandler(options)
            {
                InnerHandler = errorHandler
            };

            var httpClient = new HttpClient(authenticator)
            {
                BaseAddress = new Uri(options.BaseUrl)
            };

            return httpClient;
        }
    }
}