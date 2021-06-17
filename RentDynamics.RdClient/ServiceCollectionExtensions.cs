using System;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add <typeparamref name="TClient"/> api client type with <typeparamref name="TClientImplementation"/> as implementation
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> object to add service to</param>
        /// <param name="clientName">Name to associate <paramref name="options"/> object with</param>
        /// <param name="options">Object to get api credentials from</param>
        /// <param name="configureClient">Action allowing to extend <see cref="IHttpClientBuilder"/> configuration used to create the <see cref="HttpClient"/> instance</param>
        /// <param name="clientLifetime"><see cref="ServiceLifetime"/> of the client. Use <see cref="ServiceLifetime.Singleton"/> for console apps and <see cref="ServiceLifetime.Scoped"/> for ASPNET Core apps</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddRentDynamicsApiClient<TClient, TClientImplementation>(
            this IServiceCollection services,
            string clientName,
            RentDynamicsOptions options,
            Action<IHttpClientBuilder>? configureClient = null,
            ServiceLifetime clientLifetime = ServiceLifetime.Scoped
        )
            where TClient : class, IRentDynamicsApiClient
            where TClientImplementation : RentDynamicsApiClient, TClient
        {
            services.Configure<RentDynamicsApiClientSettings>(clientName, settings => settings.Options = options);
            services.TryAddSingleton<INonceCalculator, NonceCalculator>();
            var retryPolicyFactory = options.RetryPolicyFactory
                                  ?? RdPolicies.CreateTransientRetryPolicyFactory<TClientImplementation>(_ => RdPolicies.TransientRetryPolicy());

            var httpClientBuilder = services.AddHttpClient($"RentDynamics_{clientName}", client => client.BaseAddress = new Uri(options.BaseUrl));

            httpClientBuilder                                  //Handlers are executed from bottom to top
                .ConfigureHttpMessageHandlerBuilder(builder => //Error handler is the outer handler. We want to log error only when the last retry attempt failed
                {
                    var settings = GetClientSettings(clientName, builder);
                    builder.AdditionalHandlers.Add(ActivatorUtilities.CreateInstance<RentDynamicsHttpClientErrorHandler>(builder.Services, settings));
                })
                .ConfigureHttpMessageHandlerBuilder(builder => //Authentication is a handler in the middle. We want to update auth headers on every retry attempt.
                {
                    var settings = GetClientSettings(clientName, builder);
                    builder.AdditionalHandlers.Add(ActivatorUtilities.CreateInstance<RentDynamicsHttpClientAuthenticationHandler>(builder.Services, settings));
                })
                .AddPolicyHandler(retryPolicyFactory); //Retry policy is the inner handler is the retry policy

            configureClient?.Invoke(httpClientBuilder);

            services.Add(new ServiceDescriptor(typeof(TClient), provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                var settings = provider.GetRequiredService<IOptionsMonitor<RentDynamicsApiClientSettings>>().Get(clientName);
                return ActivatorUtilities.CreateInstance<TClientImplementation>(provider, factory.CreateClient($"RentDynamics_{clientName}"), settings);
            }, clientLifetime));

            return services;
        }

        private static RentDynamicsApiClientSettings GetClientSettings(string clientName, HttpMessageHandlerBuilder builder)
        {
            return builder.Services.GetRequiredService<IOptionsMonitor<RentDynamicsApiClientSettings>>().Get(clientName);
        }
    }
}