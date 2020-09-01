using System;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources
{
    [PublicAPI]
    public class ModelValidationException : Exception
    {
        public ModelValidationException(string message) : base(message)
        {
        }
    }
}