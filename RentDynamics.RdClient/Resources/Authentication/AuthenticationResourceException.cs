using System;

namespace RentDynamics.RdClient.Resources.Authentication
{
    public class AuthenticationResourceException : Exception
    {
        public AuthenticationResourceException(string message) : base(message)
        {
        }
    }
}