using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentDynamicsCS.DependencyInjection;
using RentDynamicsCS.HttpApiClient;
using RentDynamicsCS.Resources;

namespace RentDynamicsCS.Tests.DependencyInjection
{
    [TestClass]
    public class DependencyInjectionTests
    {
        private static IEnumerable<object[]> GetRentDynamicsResourceClasses()
        {
            var resourceClasses = typeof(BaseRentDynamicsResource).Assembly.GetTypes()
                                                                  .Where(t => !t.IsAbstract)
                                                                  .Where(t => typeof(BaseRentDynamicsResource).IsAssignableFrom(t));
            return resourceClasses.Select(resourceClass => new object[] { resourceClass });
        }

        [TestMethod]
        [DynamicData(nameof(GetRentDynamicsResourceClasses), DynamicDataSourceType.Method)]
        public void AllResourceClasses_ShouldBeResolved(Type resourceClassType)
        {
            var services = new ServiceCollection();

            services.AddDefaultRentDynamicsClient("test", "test-key", isDevelopment: true);

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            scope.ServiceProvider.GetRequiredService(resourceClassType);
        }

        [TestMethod]
        [DynamicData(nameof(GetRentDynamicsResourceClasses), DynamicDataSourceType.Method)]
        public void AllResourceClasses_ShouldBeResolved_ByResourceFactory(Type resourceClassType)
        {
            var services = new ServiceCollection();

            services.AddDefaultRentDynamicsClient("test", "test-key", isDevelopment: true);
            
            services.AddScoped<RentDynamicsApiClient>();
            services.AddScoped<RentDynamicsResourceFactory<RentDynamicsApiClient>>();

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            var factory = provider.GetRequiredService<RentDynamicsResourceFactory<RentDynamicsApiClient>>();

            var factoryMethod =  factory.GetType().GetMethod(nameof(RentDynamicsResourceFactory<RentDynamicsApiClient>.CreateResource))
                           ?? throw new Exception("CreateResource method was not found");

            object? resource = factoryMethod.MakeGenericMethod(resourceClassType).Invoke(factory, new object[0]);

            resource.Should().NotBeNull();
        }
        
    }
}