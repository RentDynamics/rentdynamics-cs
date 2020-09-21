using System;
using Microsoft.Extensions.Configuration;

namespace RentDynamics.RdClient.Tests.TestUtils
{
    public static class ConfigurationEnvExtensions
    {
        public static T GetEnvVar<T>(this IConfigurationRoot config, string envVarKey)
        {
            return config.GetValue<T>(envVarKey)
                ?? throw new Exception($"ENV variable '{envVarKey}' is missing.");
        }
    }
}