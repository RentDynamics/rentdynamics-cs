using System;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace RentDynamics.RdClient.HttpApiClient
{
    [PublicAPI]
    public static class RentDynamicsHttpClientFactory
    {
        public static HttpClient Create<TClientSettings>(TClientSettings settings, ILoggerFactory? loggerFactory)
            where TClientSettings : IRentDynamicsApiClientSettings
        {
            loggerFactory ??= new NullLoggerFactory();
            
            var httpClientHandler = new HttpClientHandler();

            var errorHandler = new RentDynamicsHttpClientErrorHandler<TClientSettings>(settings, loggerFactory.CreateLogger<RentDynamicsHttpClientErrorHandler<TClientSettings>>())
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