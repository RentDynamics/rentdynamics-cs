using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCard
{
    [PublicAPI]
    public class LeadCardVM
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
        public int? AdSourceId { get; set; }
        public int? SecondaryAdSourceId { get; set; }
        public int? LeaseTerm { get; set; }
        public int UserId { get; set; }
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

        public AddressVM? Address { get; set; }

        public List<int> Amenities { get; set; } = new List<int>();
        public List<OccupantVM> Occupants { get; set; } = new List<OccupantVM>();
        public List<PetVM> Pets { get; set; } = new List<PetVM>();

        /// <summary>
        /// The constructor for <see cref="LeadCardVM"/> class
        /// </summary>
        /// <param name="firstName">First name of the lead. This is a required parameter</param>
        /// <param name="phoneNumber">Phone number of the lead.</param>
        /// <param name="email">Email address of the lead</param>
        /// <remarks>Both <paramref name="phoneNumber"/> and <paramref name="email"/> cannot be <c>null</c> at the same time. At least one of them have to be provided.</remarks>
        [UsedImplicitly]
        public LeadCardVM(string firstName, string? phoneNumber, string? email)
        {
            FirstName = firstName;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}