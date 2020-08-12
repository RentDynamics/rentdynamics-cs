using System;

namespace RentDynamicsCS.Resources
{
    public class AuthenticationResourceException : Exception
    {
        public AuthenticationResourceException(string message) : base(message)
        {
        }
    }
}