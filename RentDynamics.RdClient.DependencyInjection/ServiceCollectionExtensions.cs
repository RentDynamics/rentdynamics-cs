using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using RentDynamics.RdClient.HttpApiClient;
using RentDynamics.RdClient.Resources;
using Scrutor;

namespace RentDynamics.RdClient.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void TryAddCoreRentDynamicsServices(this IServiceCollection services)

        {
            services.TryAddScoped<INonceCalculator, NonceCalculator>();

            services.TryAddRentDynamicsResourceFactories();

            services.Scan(selector => selector.FromAssemblyOf<BaseRentDynamicsResource>()
                                              .AddClasses(classes => classes.AssignableTo<BaseRentDynamicsResource>())
                                              .AsSelf()
                                              .UsingRegistrationStrategy(RegistrationStrategy.Skip));
        }

        private static void TryAddRentDynamicsResourceFactories(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IRentDynamicsResourceByClientFactory<>), typeof(RentDynamicsResourceByClientFactory<>));
            services.TryAddScoped(typeof(IRentDynamicsResourceBySettingsFactory<>), typeof(RentDynamicsResourceBySettingsFactory<>));
        }

        public static IServiceCollection AddRentDynamicsApiClient<TClientSettings>(this IServiceCollection services, TClientSettings settings)
            where TClientSettings : class, IRentDynamicsApiClientSettings
        {
            return services.AddRentDynamicsApiClient<IRentDynamicsApiClient<TClientSettings>, RentDynamicsApiClient<TClientSettings>, TClientSettings>(settings);
        }

        public static IServiceCollection AddRentDynamicsApiClient<TClient, TClientImplementation, TClientSettings>(
            this IServiceCollection services,
            TClientSettings settings
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

            services.AddHttpClient<TClient, TClientImplementation>((provider, client) => { client.BaseAddress = new Uri(settings.Options.BaseUrl); })
                    .AddHttpMessageHandler<RentDynamicsHttpClientErrorHandler<TClientSettings>>()
                    .AddHttpMessageHandler<RentDynamicsHttpClientAuthenticationHandler<TClientSettings>>();

            return services;
        }

        public static IServiceCollection AddDefaultRentDynamicsClient(
            this IServiceCollection services,
            string apiKey,
            string apiSecretKey,
            bool isDevelopment = false,
            JsonSerializerSettings? jsonSerializerSettings = null)
        {
            var settings = new RentDynamicsApiClientSettings
            {
                Options = new RentDynamicsOptions(apiKey, apiSecretKey, isDevelopment: isDevelopment),
                JsonSerializerSettings = jsonSerializerSettings ?? RentDynamicsDefaultSettings.DefaultSerializerSettings
            };

            return services.AddRentDynamicsApiClient<IRentDynamicsApiClient, RentDynamicsApiClient, RentDynamicsApiClientSettings>(settings);
        }

        public static IServiceCollection AddDefaultRentDynamicsClient(this IServiceCollection services, Func<IServiceProvider, IRentDynamicsApiClient> implementationFactory)
        {
            services.TryAddCoreRentDynamicsServices();
            return services.AddTransient(implementationFactory);
        }
    }
}