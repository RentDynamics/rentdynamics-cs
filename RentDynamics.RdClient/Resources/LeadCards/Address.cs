using System;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    [PublicAPI]
    public class Address
    {
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string? County { get; set; }
        public string? Zip { get; set; }

        public DateTime Created { get; private set; }

        public int? AddressTypeId { get; set; }
        
        [UsedImplicitly]
        public Address(string addressLine1, string city, string state)
        {
            AddressLine1 = addressLine1;
            City = city;
            State = state;
        }
    }
}