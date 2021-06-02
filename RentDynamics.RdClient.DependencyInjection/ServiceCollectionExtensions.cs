using System;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient.DependencyInjection
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register types required for the RdApi client.
        ///
        /// The method can be called multiple times, but it will add the required services only once.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> object to add service to</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/></returns>
        public static IServiceCollection TryAddCoreRentDynamicsServices(this IServiceCollection services)
        {
            services.TryAddScoped<INonceCalculator, NonceCalculator>();

            return services;
        }

        /// <summary>
        /// Add <typeparamref name="TClient"/> api client type with <typeparamref name="TClientImplementation"/> as implementation type using <typeparamref name="TClientSettings"/> type to get api credentials from
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> object to add service to</param>
        /// <param name="clientName">Name to associate <paramref name="options"/> object with</param>
        /// <param name="options">Object to get api credentials from</param>
        /// <param name="configureClient">Action allowing to extend <see cref="IHttpClientBuilder"/> configuration used to create the <see cref="HttpClient"/> instance</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddRentDynamicsApiClient<TClient, TClientImplementation>(
            this IServiceCollection services,
            string clientName,
            RentDynamicsOptions options,
            Action<IHttpClientBuilder>? configureClient = null
        )
            where TClient : class, IRentDynamicsApiClient
            where TClientImplementation : RentDynamicsApiClient, TClient
        {
            services.Configure<RentDynamicsApiClientSettings>(clientName, settings => settings.Options = options);
            services.TryAddCoreRentDynamicsServices();

            var httpClientBuilder = services.AddHttpClient($"RentDynamics_{clientName}", client => client.BaseAddress = new Uri(options.BaseUrl));

            httpClientBuilder
                .ConfigureHttpMessageHandlerBuilder(builder =>
                {
                    RentDynamicsApiClientSettings settings = builder.Services.GetRequiredService<IOptionsMonitor<RentDynamicsApiClientSettings>>().Get(clientName);
                    builder.AdditionalHandlers.Add(ActivatorUtilities.CreateInstance<RentDynamicsHttpClientAuthenticationHandler>(builder.Services, settings));
                    builder.AdditionalHandlers.Add(ActivatorUtilities.CreateInstance<RentDynamicsHttpClientErrorHandler>(builder.Services, settings));
                });

            configureClient?.Invoke(httpClientBuilder);

            services.AddScoped<TClient, TClientImplementation>(provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                var settings = provider.GetRequiredService<IOptionsMonitor<RentDynamicsApiClientSettings>>().Get(clientName);
                return ActivatorUtilities.CreateInstance<TClientImplementation>(provider, factory.CreateClient($"RentDynamics_{clientName}"), settings);
            });

            return services;
        }
    }
}