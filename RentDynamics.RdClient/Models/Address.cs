using System;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Models
{
    [PublicAPI]
    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }

        public DateTime Created { get; private set; }

        public int? AddressTypeId { get; set; }
    }
}