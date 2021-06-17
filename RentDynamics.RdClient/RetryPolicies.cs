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
        
        /// <summary>
        /// Dictionary key under which a <see cref="ILogger"/> instance is stored in Polly's <see cref="Context"/>
        /// </summary>
        public const string LoggerKey = nameof(LoggerKey);

        /// <summary>
        /// Creates a policy that automatically retries HTTP requests that failed with transient errors
        /// <para>Retry conditions:
        /// <list type="bullet">
        /// <item><description>Network failures (as <see cref="T:System.Net.Http.HttpRequestException" />)</description></item>
        /// <item><description>HTTP 5XX status codes (server errors)</description></item>
        /// <item><description>HTTP 408 status code (request timeout)</description></item>
        /// </list>
        /// </para> 
        /// </summary>
        /// <param name="medianFirstRetry">Median value used to calculate delay timeouts with <see cref="Backoff.DecorrelatedJitterBackoffV2"/> strategy</param>
        /// <param name="retryAttempts">Number of retry attempts</param>
        /// <returns></returns>
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

        /// <summary>
        /// Shortcut to call <see cref="TransientRetryPolicy(System.TimeSpan,int)"/> using default parameters:
        /// <see cref="DefaultMedianRetryDelay"/> and <see cref="DefaultMedianRetryDelay"/>
        /// </summary>
        /// <returns></returns>
        public static IAsyncPolicy<HttpResponseMessage> TransientRetryPolicy()
            => TransientRetryPolicy(DefaultMedianRetryDelay, DefaultRetryCount);

        /// <summary>
        /// Checks whether transient retry policy is enabled for a given <paramref name="message"/>
        /// by checking its <see cref="HttpRequestMessage.Properties"/> for <see cref="HttpRequestMessageProperties.UseTransientRetryPolicyForRequest"/> key presense
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ApplyTransientRetryPolicy(HttpRequestMessage message)
        {
            return message.Properties.TryGetValue(HttpRequestMessageProperties.UseTransientRetryPolicyForRequest, out object? useRetryPolicy)
                && useRetryPolicy is true;
        }


        /// <summary>
        /// Builds a top-level factory for constructing transient retry-policies.
        /// <para>
        /// Transient retry policy is enabled only for specific methods. See <see cref="ApplyTransientRetryPolicy"/> for details.
        /// </para>
        /// <para>
        /// The factory will populate <see cref="Context"/> found in <see cref="HttpRequestMessage.Properties"/> with
        /// a <see cref="ILogger{TCategoryName}"/> instance to allow using logger in policies. 
        /// </para>
        /// </summary>
        /// <param name="policyProvider">Factory that creates the actual transient retry policy</param>
        /// <typeparam name="TLoggerCategory">Type to use as category name for <see cref="ILogger{TCategoryName}"/></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Shortcut for <see cref="CreateTransientRetryPolicyFactory{TLoggerCategory}"/> that uses <see cref="RentDynamicsApiClient"/>
        /// as category for <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        /// <returns></returns>
        public static Func<IServiceProvider, HttpRequestMessage, IAsyncPolicy<HttpResponseMessage>> CreateTransientRetryPolicyFactory(
            Func<IServiceProvider, IAsyncPolicy<HttpResponseMessage>> policyProvider)
            => CreateTransientRetryPolicyFactory<RentDynamicsApiClient>(policyProvider);
    }
}