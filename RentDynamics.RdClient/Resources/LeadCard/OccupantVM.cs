using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCard
{
    [PublicAPI]
    public class OccupantVM
    {
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string? LastName { get; set; }
        public string EmailAddress { get; set; }

        public long PersonId { get; set; }
        public long PrimaryLeadId { get; set; }
        public bool IsLessee { get; set; }

        public int? RelatedPersonId { get; set; }
        public int? RelationshipTypeId { get; set; }

//Disable nullability warnings
#pragma warning disable 8618
        [UsedImplicitly]
        protected OccupantVM() //Protected ctor for deserialization only.
                               //This ctor is required because API sometimes returns this object with 'relationshipTypeId' property set to 'null'
                               //But 'relationshipTypeId' must be a non-null value when the object is sent to API 
        {
        }
#pragma warning restore 8618
        
        public OccupantVM(string firstName, string phoneNumber, string emailAddress, int relationshipTypeId)
        {
            FirstName = firstName;
            RelationshipTypeId = relationshipTypeId;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
        }
    }
}