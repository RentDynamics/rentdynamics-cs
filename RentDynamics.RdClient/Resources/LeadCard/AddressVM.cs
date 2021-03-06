using System;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCard
{
    [PublicAPI]
    public class AddressVM
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
        public AddressVM(string addressLine1, string city, string state)
        {
            AddressLine1 = addressLine1;
            City = city;
            State = state;
        }
    }
}