namespace RentDynamics.RdClient.HttpApiClient
{
    public static class HttpRequestMessageProperties
    {
        private const string Prefix = "RentDynamics_";

        public const string UseTransientRetryPolicyForRequest = Prefix + nameof(UseTransientRetryPolicyForRequest);
    }
}