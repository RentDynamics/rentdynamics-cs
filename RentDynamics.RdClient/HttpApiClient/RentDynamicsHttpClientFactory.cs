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
        public static HttpClient Create<TClientSettings>(
            TClientSettings settings,
            ILoggerFactory? loggerFactory = null,
            DelegatingHandler? outerHandler = null)
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

            if (outerHandler == null)
            {
                outerHandler = authenticator;
            }
            else
            {
                SetInnerHandler(outerHandler, authenticator);
            }

            var httpClient = new HttpClient(outerHandler)
            {
                BaseAddress = new Uri(settings.Options.BaseUrl)
            };

            return httpClient;
        }

        private static void SetInnerHandler(DelegatingHandler outerHandler, DelegatingHandler innerHandler)
        {
            var current = outerHandler;
            while (current.InnerHandler != null)
            {
                if (!(current.InnerHandler is DelegatingHandler innerDelegatingHandler))
                {
                    throw new RentDynamicsHttpClientFactoryException($"Only {typeof(DelegatingHandler)} handler types are supported. Received: {current.InnerHandler.GetType()}");
                }

                current = innerDelegatingHandler;
            }
            
            current.InnerHandler = innerHandler;
        }
    }
}