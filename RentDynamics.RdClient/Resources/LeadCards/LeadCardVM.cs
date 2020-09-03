using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCards
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
        public List<PetVM> Pets { get;  set; } = new List<PetVM>();

        
#pragma warning disable 8618
        [UsedImplicitly]
        protected LeadCardVM() //Ctor for deserialization only. Required to by-pass validation implemented by the public ctor. 
        {
        }
#pragma warning restore 8618

        public LeadCardVM(string firstName, string? phoneNumber, string? email)
        {
            if (phoneNumber == null && email == null)
            {
                throw new ModelValidationException($"At least {nameof(phoneNumber)} or {nameof(email)} must be specified");
            }
            
            FirstName = firstName;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}