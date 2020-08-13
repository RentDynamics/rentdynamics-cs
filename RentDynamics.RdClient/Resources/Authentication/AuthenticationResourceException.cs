using System;

namespace RentDynamics.RdClient.Resources
{
    public class AuthenticationResourceException : Exception
    {
        public AuthenticationResourceException(string message) : base(message)
        {
        }
    }
}