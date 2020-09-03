using System;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    [PublicAPI]
    public class PetVM
    {
        public int PetTypeId { get; set; }
        public string? Breed { get; set; }
        public string? PetName { get; set; }
        public bool IsServiceAnimal { get; set; }
        public long? CreatedById { get; set; }
        public long? LeadId { get; set; }
        public DateTime Created { get; protected set; }

        [UsedImplicitly]
        public PetVM(int petTypeId)
        {
            PetTypeId = petTypeId;
        }
    }
}