using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using RentDynamicsCS.HttpApiClient;
using RentDynamicsCS.Resources;
using Scrutor;

namespace RentDynamicsCS.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static void TryAddCoreRentDynamicsServices(this IServiceCollection services)

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
            services.TryAddScoped(typeof(IRentDynamicsResourceFactory<>), typeof(RentDynamicsResourceFactory<>));
            services.TryAddScoped<IRentDynamicsResourceFactory>(provider => provider.GetRequiredService<IRentDynamicsResourceFactory<IRentDynamicsApiClient>>());
        }

        public static IServiceCollection AddRentDynamicsApiClient<TClient, TClientImplementation, TClientSettings>(
            this IServiceCollection services,
            TClientSettings settings
        )
            where TClient : class, IRentDynamicsApiClient
            where TClientSettings : class, IRentDynamicsApiClientSettings
            where TClientImplementation : RentDynamicsApiClient<TClientSettings>, TClient
        {
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
            return services.AddTransient(implementationFactory);
        }
    }
}