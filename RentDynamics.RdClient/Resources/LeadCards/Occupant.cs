using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    [PublicAPI]
    public class Occupant
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }

        public long PersonId { get; set; }
        public long PrimaryLeadId { get; set; }
        public bool IsLessee { get; set; }

        public int? RelatedPersonId { get; set; }
        public int? RelationshipTypeId { get; set; }
    }
}