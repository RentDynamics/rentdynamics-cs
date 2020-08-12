using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace RentDynamicsCS.HttpApiClient
{
    public static class RentDynamicsHttpClientFactory
    {
        public static HttpClient Create<TClientSettings>(TClientSettings settings)
            where TClientSettings : IRentDynamicsApiClientSettings
        {
            var httpClientHandler = new HttpClientHandler();

            var errorHandler = new RentDynamicsHttpClientErrorHandler<TClientSettings>(settings)
            {
                InnerHandler = httpClientHandler
            };

            var authenticator = new RentDynamicsHttpClientAuthenticationHandler<TClientSettings>(settings)
            {
                InnerHandler = errorHandler
            };

            var httpClient = new HttpClient(authenticator)
            {
                BaseAddress = new Uri(settings.Options.BaseUrl)
            };

            return httpClient;
        }
    }
}