using System;

namespace RentDynamics.Client.Resources
{
    public class AuthenticationResourceException : Exception
    {
        public AuthenticationResourceException(string message) : base(message)
        {
        }
    }
}