using System;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentDynamics.RdClient.HttpApiClient;
using RentDynamics.RdClient.Resources;
using RentDynamics.RdClient.Tests.TestUtils;

namespace RentDynamics.RdClient.Tests.IntegrationTests
{
    [TestClass, TestCategory("RD-API-IntegrationTests")]
    public class BaseIntegrationTest
    {
        protected IConfigurationRoot Config { get; }

        protected RentDynamicsOptions FullAccessOptions { get; private set; } = null!;

        protected int AvailableCommunityGroupId => Config.GetEnvVar<int>("RD_API_COMMUNITY_GROUP_ID");
        protected string RdApiUserName => Config.GetEnvVar<string>("RD_API_USERNAME");
        protected string RdApiPassword => Config.GetEnvVar<string>("RD_API_PASSWORD");
        
        protected virtual bool AutomaticAuthentication => Config.GetValue<bool?>("RD_API_AUTO_AUTHENTICATION") == true;

        public BaseIntegrationTest()
        {
            Config = new ConfigurationBuilder().AddEnvironmentVariables()
                                               .Build();

            FullAccessOptions = new RentDynamicsOptions(Config.GetEnvVar<string>("RD_API_KEY"),
                                                        Config.GetEnvVar<string>("RD_API_SECRET_KEY"),
                                                        isDevelopment: true);

            if (AutomaticAuthentication)
            {
                Console.WriteLine("Start login");

                RentDynamicsApiClient client = CreateApiClient();
                var authenticationResource = new AuthenticationResource(client);

                authenticationResource.Login(RdApiUserName, RdApiPassword);
                
                Console.WriteLine("Finish login");

            }
        }

        protected virtual RentDynamicsApiClient CreateApiClient()
        {
            var settings = new RentDynamicsApiClientSettings { Options = FullAccessOptions };
            var client = new RentDynamicsApiClient(settings);
            return client;
        }
    }
}