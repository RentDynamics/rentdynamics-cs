using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RentDynamicsCS.HttpApiClient;
using RentDynamicsCS.Resources;

namespace RentDynamicsCS.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRentDynamicsApi(this IServiceCollection services, RentDynamicsOptions options, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            services.AddSingleton<RentDynamicsOptions>(_ => options);
            services.AddSingleton<RentDynamicsApiClientSettings>(provider => new RentDynamicsApiClientSettings
            {
                Options = provider.GetRequiredService<RentDynamicsOptions>(),
                JsonSerializerSettings = jsonSerializerSettings ?? RentDynamicsDefaultSettings.DefaultSerializerSettings
            });

            services.AddScoped<RentDynamicsHttpClientErrorHandler>(_ => new RentDynamicsHttpClientErrorHandler(jsonSerializerSettings ??
                                                                                                               RentDynamicsDefaultSettings.DefaultSerializerSettings));
            services.AddScoped<RentDynamicsHttpClientAuthenticationHandler>();
            services.AddScoped<INonceCalculator, NonceCalculator>();

            services.AddHttpClient<IRentDynamicsApiClient, RentDynamicsApiClient>((provider, client) =>
                    {
                        var options = provider.GetRequiredService<RentDynamicsOptions>();
                        client.BaseAddress = new Uri(options.BaseUrl);
                    })
                    .AddHttpMessageHandler<RentDynamicsHttpClientErrorHandler>()
                    .AddHttpMessageHandler<RentDynamicsHttpClientAuthenticationHandler>();

            services.Scan(selector => selector.FromAssemblyOf<BaseRentDynamicsResource>()
                                              .AddClasses(classes => classes.AssignableTo<BaseRentDynamicsResource>())
                                              .AsImplementedInterfaces());

            return services;

        }
        public static IServiceCollection AddRentDynamicsApi(
            this IServiceCollection services,
            string apiKey,
            string apiSecretKey,
            bool isDevelopment = false,
            JsonSerializerSettings? jsonSerializerSettings = null)
        {
            return services.AddRentDynamicsApi(new RentDynamicsOptions(apiKey, apiSecretKey, isDevelopment: isDevelopment), jsonSerializerSettings);
        }
    }
}