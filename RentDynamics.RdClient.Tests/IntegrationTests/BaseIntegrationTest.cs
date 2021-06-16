using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentDynamics.RdClient.HttpApiClient;
using RentDynamics.RdClient.Resources.Authentication;
using RentDynamics.RdClient.Tests.TestUtils;

namespace RentDynamics.RdClient.Tests.IntegrationTests
{
    [TestClass, TestCategory("RD-API-IntegrationTests")]
    public class BaseRdApiIntegrationTest
    {
        private IServiceProvider _serviceProvider;
        
        protected IConfigurationRoot Config { get; }

        protected RentDynamicsOptions ApiOptions { get; }

        protected virtual int AvailableCommunityGroupId => Config.GetEnvVar<int>("RD_API_COMMUNITY_GROUP_ID");
        protected virtual int AvailableCommunityId => Config.GetEnvVar<int>("RD_API_COMMUNITY_ID");
        protected virtual string RdApiUserName => Config.GetEnvVar<string>("RD_API_USERNAME");
        protected virtual string RdApiPassword => Config.GetEnvVar<string>("RD_API_PASSWORD");
        protected virtual string RdApiKey => Config.GetEnvVar<string>("RD_API_KEY");
        protected virtual string RdApiSecretKey => Config.GetEnvVar<string>("RD_API_SECRET_KEY");
        protected virtual bool AutomaticAuthentication => Config.GetValue<bool?>("RD_API_AUTO_AUTHENTICATION") == true;

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public BaseRdApiIntegrationTest()
        {
            Config = new ConfigurationBuilder().AddEnvironmentVariables()
                                               .Build();

            ApiOptions = new RentDynamicsOptions(RdApiKey, RdApiSecretKey, isDevelopment: true);

            _serviceProvider = new ServiceCollection()
                               .AddRentDynamicsApiClient<IRentDynamicsApiClient, RentDynamicsApiClient>("TestClient", ApiOptions)
                               .BuildServiceProvider();

            if (AutomaticAuthentication)
            {
                Console.WriteLine("Start login");

                var client = CreateApiClient();
                var authenticationResource = new AuthenticationResource(client);

                authenticationResource.Login(RdApiUserName, RdApiPassword);

                Console.WriteLine("Finish login");
            }
        }

        protected virtual IRentDynamicsApiClient CreateApiClient()
        {
            return _serviceProvider.GetRequiredService<IRentDynamicsApiClient>();
        }
    }
}