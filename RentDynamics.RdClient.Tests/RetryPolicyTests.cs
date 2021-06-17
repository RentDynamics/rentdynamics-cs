using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Contrib.HttpClient;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient.Tests
{
  [TestClass]
  public class RetryPolicyTests
  {
    private static IRentDynamicsApiClient? CreateClient(HttpMessageHandler handler, int retryCount)
    {
      TimeSpan mediumRetryDelay = TimeSpan.FromMilliseconds(10);
      var options = new RentDynamicsOptions("test", "test")
      {
        RetryPolicyFactory = RdPolicies.CreateTransientRetryPolicyFactory(_ => RdPolicies.TransientRetryPolicy(mediumRetryDelay, retryCount))
      };
      var provider = new ServiceCollection()
                     .AddRentDynamicsApiClient<IRentDynamicsApiClient, RentDynamicsApiClient>(
                       "default", options,
                       builder => builder.ConfigurePrimaryHttpMessageHandler(() => handler),
                       clientLifetime: ServiceLifetime.Singleton)
                     .BuildServiceProvider();
      return provider.GetRequiredService<IRentDynamicsApiClient>();
    }

    private static async Task RunTestAsync(int expectedAttempts, HttpMethod method, Func<IRentDynamicsApiClient, bool ,Task> action)
    {
      bool useTransientRetryPolicy = expectedAttempts > 1;
      int retryCount = expectedAttempts - 1;

      var mockHandler = new Mock<HttpMessageHandler>();
      mockHandler.SetupRequest(method, _ => true).Throws(new HttpRequestException("Test transient exception"));

      IRentDynamicsApiClient? client = CreateClient(mockHandler.Object, retryCount);

      await client.Invoking(x => action(x!, useTransientRetryPolicy))
                  .Should()
                  .ThrowExactlyAsync<HttpRequestException>();

      mockHandler.VerifyRequest(method, _ => true, Times.Exactly(expectedAttempts));
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public async Task RetryPolicy_ShouldExecuteCorrectNumberOrAttempts_OnGetRequest(int expectedAttempts)
    {
      await RunTestAsync(expectedAttempts, 
                         HttpMethod.Get, 
                         (client, useRetryPolicy) => client.GetAsync<object>("", useTransientRetryPolicy: useRetryPolicy));
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public async Task RetryPolicy_ShouldExecuteCorrectNumberOrAttempts_OnPostRequest(int expectedAttempts)
    {
      await RunTestAsync(expectedAttempts,
                         HttpMethod.Post,
                         (client, useRetryPolicy) => client.PostAsync<object, object>("", new object(), useTransientRetryPolicy: useRetryPolicy));
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public async Task RetryPolicy_ShouldExecuteCorrectNumberOrAttempts_OnPutRequest(int expectedAttempts)
    {
      await RunTestAsync(expectedAttempts,
                         HttpMethod.Put,
                         (client, useRetryPolicy) => client.PutAsync<object, object>("", new object(), useTransientRetryPolicy: useRetryPolicy));
    }
    
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public async Task RetryPolicy_ShouldExecuteCorrectNumberOrAttempts_OnDeleteRequest(int expectedAttempts)
    {
      await RunTestAsync(expectedAttempts,
                         HttpMethod.Delete,
                         (client, useRetryPolicy) => client.DeleteAsync("", useTransientRetryPolicy: useRetryPolicy));
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public async Task RetryPolicy_ShouldExecuteCorrectNumberOrAttempts_OnDeleteRequest2(int expectedAttempts)
    {
      await RunTestAsync(expectedAttempts,
                         HttpMethod.Delete,
                         (client, useRetryPolicy) => client.DeleteAsync<object>("", useTransientRetryPolicy: useRetryPolicy));
    }

  }
}