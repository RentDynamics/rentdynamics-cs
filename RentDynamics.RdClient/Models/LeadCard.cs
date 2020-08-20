using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Models
{
    [PublicAPI]
    public class LeadCard
    {
        public string FirstName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public int? Bedrooms { get; set; }
        public decimal? Bathrooms { get; set; }
        public string? LastName { get; set; }
        public string? Note { get; set; }
        public bool? TrySms { get; private set; }

        public DateTime? MoveDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime Created { get; private set; }

        public long LeadId { get; set; }
        public long? AdSourceId { get; set; }
        public int? SecondaryAdSourceId { get; set; }
        public int? LeaseTerm { get; set; }
        public long UserId { get; set; }
        public int? ReferrerSourceId { get; set; }

        public int Community { get; private set; }

        public int? MoveReasonTypeId { get; private set; }
        public int? NoAppointmentReasonTypeId { get; set; }
        public int? UnqualifiedReasonTypeId { get; set; }
        public int? EndFollowUpReasonTypeId { get; set; }
        public int? TourTypeId { get; set; }
        public int? CommunicationTypeId { get; set; }
        public int? PreferredCommunicationTypeId { get; set; }
        public int? SecondaryPreferredCommunicationTypeId { get; set; }

        public Address? Address { get; set; }

        public List<int> Amenities { get; set; } = new List<int>();
        public List<Occupant> Occupants { get; set; } = new List<Occupant>();
        public List<Pet> Pets { get;  set; } = new List<Pet>();
    }
}