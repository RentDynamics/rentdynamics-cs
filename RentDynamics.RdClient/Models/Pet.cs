using JetBrains.Annotations;

namespace RentDynamics.RdClient.Models
{
    [PublicAPI]
    public class Pet
    {
        public long LeadId { get; set; }
        public int PetTypeId { get; set; }
        public string Breed { get; set; }
        public string PetName { get; set; }
        public bool IsServiceAnimal { get; set; }
        public long? CreatedBy { get; set; }
    }
}