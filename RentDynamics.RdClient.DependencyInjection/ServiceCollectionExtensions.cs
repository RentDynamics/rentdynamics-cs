using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using RentDynamics.RdClient.HttpApiClient;
using RentDynamics.RdClient.Resources;
using Scrutor;

namespace RentDynamics.RdClient.DependencyInjection
{
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        private static IServiceTypeSelector SelectRentDynamicsResources(this IImplementationTypeSelector selector)
        {
            return selector
                   .AddClasses(classes => classes.AssignableTo<BaseRentDynamicsResource>())
                   .AsSelf()
                   .UsingRegistrationStrategy(RegistrationStrategy.Skip);
        }

        /// <summary>
        /// Add classes that inherit from <see cref="BaseRentDynamicsResource"/> to <see cref="IServiceCollection"/> defined in <paramref name="assembliesToScan"/>
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> object to add service to</param>
        /// <param name="assembliesToScan">List of assemblies to search</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddRentDynamicsResourceClasses(this IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            return services.Scan(selector => selector.FromAssemblies(assembliesToScan).SelectRentDynamicsResources());
        }

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

            services.TryAddRentDynamicsResourceFactories();

            services.Scan(selector => selector.FromAssemblyOf<BaseRentDynamicsResource>().SelectRentDynamicsResources());

            return services;
        }

        private static void TryAddRentDynamicsResourceFactories(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IRentDynamicsResourceByClientFactory<>), typeof(RentDynamicsResourceByClientFactory<>));
            services.TryAddScoped(typeof(IRentDynamicsResourceBySettingsFactory<>), typeof(RentDynamicsResourceBySettingsFactory<>));
        }

        public static IServiceCollection AddRentDynamicsApiClient<TClientSettings>(
            this IServiceCollection services,
            TClientSettings settings,
            Action<IHttpClientBuilder>? configureClient = null)
            where TClientSettings : class, IRentDynamicsApiClientSettings
        {
            return services.AddRentDynamicsApiClient<IRentDynamicsApiClient<TClientSettings>, RentDynamicsApiClient<TClientSettings>, TClientSettings>(settings, configureClient);
        }

        /// <summary>
        /// Add <typeparamref name="TClient"/> api client type with <typeparamref name="TClientImplementation"/> as implementation type using <typeparamref name="TClientSettings"/> type to get api credentials from
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> object to add service to</param>
        /// <param name="settings">Object to get api credentials from</param>
        /// <param name="configureClient">Action allowing to extend <see cref="IHttpClientBuilder"/> configuration used to create the <see cref="HttpClient"/> instance</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddRentDynamicsApiClient<TClient, TClientImplementation, TClientSettings>(
            this IServiceCollection services,
            TClientSettings settings,
            Action<IHttpClientBuilder>? configureClient = null
        )
            where TClient : class, IRentDynamicsApiClient
            where TClientSettings : class, IRentDynamicsApiClientSettings
            where TClientImplementation : RentDynamicsApiClient<TClientSettings>, TClient
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (settings.Options == null) throw new ArgumentNullException(nameof(settings.Options));

            services.AddSingleton(settings);
            services.TryAddCoreRentDynamicsServices();

            services.AddScoped<RentDynamicsHttpClientErrorHandler<TClientSettings>>();
            services.AddScoped<RentDynamicsHttpClientAuthenticationHandler<TClientSettings>>();

            IHttpClientBuilder httpClientBuilder = services.AddHttpClient<TClient, TClientImplementation>((provider, client) => { client.BaseAddress = new Uri(settings.Options.BaseUrl); });
            
            httpClientBuilder.AddHttpMessageHandler<RentDynamicsHttpClientErrorHandler<TClientSettings>>()
                             .AddHttpMessageHandler<RentDynamicsHttpClientAuthenticationHandler<TClientSettings>>();
            
            configureClient?.Invoke(httpClientBuilder);

            return services;
        }

        /// <summary>
        /// Add <see cref="IRentDynamicsApiClient"/> interface implementation to the <see cref="IServiceCollection"/> with all the required services
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> object to add service to</param>
        /// <param name="apiKey">Api key used to access RentDynamics API</param>
        /// <param name="apiSecretKey">Api secret key used to access RentDynamics API</param>
        /// <param name="isDevelopment">Use development RentDynamics API server instead of production</param>
        /// <param name="jsonSerializerSettings">Custom <see cref="JsonSerializerSettings"/>. This field is optional. It is recommended to use the default settings.</param>
        /// <param name="configureClient">Action allowing to extend <see cref="IHttpClientBuilder"/> configuration used to create the <see cref="HttpClient"/> instance</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddDefaultRentDynamicsClient(
            this IServiceCollection services,
            string apiKey,
            string apiSecretKey,
            bool isDevelopment = false,
            JsonSerializerSettings? jsonSerializerSettings = null,
            Action<IHttpClientBuilder>? configureClient = null)
        {
            var options = new RentDynamicsOptions(apiKey, apiSecretKey, isDevelopment: isDevelopment);
            var settings = new RentDynamicsApiClientSettings(options, jsonSerializerSettings);

            return services.AddRentDynamicsApiClient<IRentDynamicsApiClient, RentDynamicsApiClient, RentDynamicsApiClientSettings>(settings, configureClient);
        }

        public static IServiceCollection AddDefaultRentDynamicsClient(this IServiceCollection services, Func<IServiceProvider, IRentDynamicsApiClient> implementationFactory)
        {
            services.TryAddCoreRentDynamicsServices();
            return services.AddTransient(implementationFactory);
        }
    }
}