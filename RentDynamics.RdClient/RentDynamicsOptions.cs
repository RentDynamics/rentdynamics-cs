using System;

namespace RentDynamics.RdClient
{
    public class RentDynamicsOptions
    {
        public string ApiKey { get; }
        public string ApiSecretKey { get; }
        public UserAuthentication UserAuthentication { get; }

        public bool IsDevelopment { get; }
        public string DevelopmentUrl { get; }
        public string ProductionUrl { get; }

        public string BaseUrl => IsDevelopment ? DevelopmentUrl : ProductionUrl;

        public RentDynamicsOptions(
            string apiKey,
            string apiSecretKey,
            Action<UserAuthentication>? configureUserAuthentication = null,
            bool isDevelopment = false,
            string productionUrl = "https://api.rentdynamics.com",
            string developmentUrl = "https://api-dev.rentdynamics.com")
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            ApiSecretKey = apiSecretKey ?? throw new ArgumentNullException(nameof(apiSecretKey));
            UserAuthentication = new UserAuthentication();
            IsDevelopment = isDevelopment;
            DevelopmentUrl = developmentUrl;
            ProductionUrl = productionUrl;

            configureUserAuthentication?.Invoke(UserAuthentication);
        }
    }
}