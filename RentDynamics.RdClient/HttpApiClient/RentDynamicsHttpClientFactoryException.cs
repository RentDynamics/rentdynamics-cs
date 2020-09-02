using System;

namespace RentDynamics.RdClient.HttpApiClient
{
    public class RentDynamicsHttpClientFactoryException : Exception
    {
        public RentDynamicsHttpClientFactoryException(string message) : base(message)
        {
        }
    }
}