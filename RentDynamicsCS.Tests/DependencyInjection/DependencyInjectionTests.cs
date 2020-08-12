using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentDynamicsCS.DependencyInjection;
using RentDynamicsCS.Resources;

namespace RentDynamicsCS.Tests.DependencyInjection
{
    [TestClass]
    public class DependencyInjectionTests
    {
        private static IEnumerable<object[]> GetRentDynamicsResourceInterfaces()
        {
            var resourceClasses = typeof(BaseRentDynamicsResource).Assembly.GetTypes()
                                                                  .Where(t => !t.IsAbstract)
                                                                  .Where(t => typeof(BaseRentDynamicsResource).IsAssignableFrom(t));
            foreach (Type resource in resourceClasses)
            {
                var implementedInterfaces = resource.GetTypeInfo().ImplementedInterfaces;
                foreach (var resourceInterface in implementedInterfaces)
                {
                    if (resourceInterface.GetTypeInfo().IsGenericType) yield break; //Skip generic types

                    yield return new object[] { resourceInterface };
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetRentDynamicsResourceInterfaces), DynamicDataSourceType.Method)]
        public void AllResourceInterfaces_ShouldBeResolved(Type resourceInterfaceType)
        {
            var services = new ServiceCollection();

            services.AddDefaultRentDynamicsClient("test", "test-key", isDevelopment: true);

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            scope.ServiceProvider.GetRequiredService(resourceInterfaceType);
        }
    }
}