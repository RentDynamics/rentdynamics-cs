using System;
using System.Net.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient
{
    [PublicAPI]
    public static class RdPolicies
    {
        public static readonly TimeSpan DefaultMedianRetryDelay = TimeSpan.FromSeconds(3);
        public const int DefaultRetryCount = 3;
        
        public const string LoggerKey = nameof(LoggerKey);

        public static IAsyncPolicy<HttpResponseMessage> TransientRetryPolicy(TimeSpan medianFirstRetry, int retryAttempts)
        {
            var delays = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetry, retryAttempts);

            return HttpPolicyExtensions
                   .HandleTransientHttpError()
                   .WaitAndRetryAsync(delays, (result, delay, attempt, context) =>
                   {
                       if (!(context.TryGetValue(LoggerKey, out var loggerValue) &&
                             loggerValue is ILogger logger))
                           return;

                       if (result.Exception != null)
                       {
                           logger.LogWarning(result.Exception, "HTTP request failed with exception, making {retryAttempt} retry after {retryAttemptDelay} delay", attempt, delay);
                       }
                       else if (!result.Result.IsSuccessStatusCode)
                       {
                           logger.LogWarning(
                               result.Exception, "HTTP response has non-success code {retryHttpResponseCode}, making {retryAttempt} retry after {retryAttemptDelay} delay",
                               (int) result.Result.StatusCode, attempt, delay);
                       }
                   });
        }

        public static IAsyncPolicy<HttpResponseMessage> TransientRetryPolicy()
            => TransientRetryPolicy(DefaultMedianRetryDelay, DefaultRetryCount);

        public static bool ApplyTransientRetryPolicy(HttpRequestMessage message)
        {
            return message.Properties.TryGetValue(HttpRequestMessageProperties.UseTransientRetryPolicyForRequest, out object? useRetryPolicy)
                && useRetryPolicy is true;
        }


        public static Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> CreateTransientRetryPolicyFactory<TLoggerCategory>(
            Func<IServiceProvider, IAsyncPolicy<HttpResponseMessage>> policyProvider)
        {
            return (provider, message) =>
            {
                if (message.Properties.TryGetValue("PolicyExecutionContext", out var policyContextValue)
                 && policyContextValue is Context policyContext)
                {
                    policyContext.Add(LoggerKey, provider.GetService<ILogger<TLoggerCategory>>());
                }

                return ApplyTransientRetryPolicy(message)
                    ? policyProvider(provider)
                    : Policy.NoOpAsync<HttpResponseMessage>();
            };
        }

        public static Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> CreateTransientRetryPolicyFactory(
            Func<IServiceProvider, IAsyncPolicy<HttpResponseMessage>> policyProvider)
            => CreateTransientRetryPolicyFactory<RentDynamicsApiClient>(policyProvider);
    }
}