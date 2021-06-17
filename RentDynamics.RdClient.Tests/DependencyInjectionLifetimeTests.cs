using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentDynamics.RdClient.HttpApiClient;

namespace RentDynamics.RdClient.Tests
{
  [TestClass]
  public class DependencyInjectionLifetimeTests
  {
    private RentDynamicsOptions Options => new RentDynamicsOptions("test", "test");

    private ServiceProvider? CreateProvider(ServiceLifetime clientLifetime)
    {
      return new ServiceCollection()
             .AddRentDynamicsApiClient<IRentDynamicsApiClient, RentDynamicsApiClient>("default", Options, clientLifetime: clientLifetime)
             .BuildServiceProvider(validateScopes: true);
    }

    [TestMethod]
    public void ApiClient_ShouldBeResolved_WithTransientLifetime()
    {
      ServiceProvider? provider = CreateProvider(ServiceLifetime.Transient);
      var client = provider.GetRequiredService<IRentDynamicsApiClient>();
      client.Should().NotBeNull();
    }

    [TestMethod]
    public void ApiClient_ShouldBeResolved_WithSingletonLifetime()
    {
      ServiceProvider? provider = CreateProvider(ServiceLifetime.Singleton);
      var client = provider.GetRequiredService<IRentDynamicsApiClient>();
      client.Should().NotBeNull();
    }

    [TestMethod]
    public void ApiClient_ShouldBeResolved_WithScopedLifetime()
    {
      ServiceProvider? provider = CreateProvider(ServiceLifetime.Scoped);
      using IServiceScope scope = provider.CreateScope();
      var client = scope.ServiceProvider.GetRequiredService<IRentDynamicsApiClient>();
      client.Should().NotBeNull();
    }
  }
}