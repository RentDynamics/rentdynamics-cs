using System;

namespace RentDynamicsCS
{
    public class RentDynamicsOptions
    {
        public string ApiKey { get; }
        public string ApiSecretKey { get; }
        public string? AuthToken { get; set; }

        public bool IsDevelopment { get; }
        public string DevelopmentUrl { get; }
        public string ProductionUrl { get; }

        public string BaseUrl => IsDevelopment ? DevelopmentUrl : ProductionUrl;

        public RentDynamicsOptions(
            string apiKey,
            string apiSecretKey,
            string? authToken = null,
            bool isDevelopment = false,
            string productionUrl = "https://api.rentdynamics.com",
            string developmentUrl = "https://api-dev.rentdynamics.com")
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            ApiSecretKey = apiSecretKey ?? throw new ArgumentNullException(nameof(apiSecretKey));
            AuthToken = authToken;
            IsDevelopment = isDevelopment;
            DevelopmentUrl = developmentUrl;
            ProductionUrl = productionUrl;
        }
    }
}