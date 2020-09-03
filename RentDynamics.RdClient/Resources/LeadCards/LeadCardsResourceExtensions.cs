using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    [PublicAPI]
    public static class LeadCardsResourceExtensions
    {
        public static LeadCardVM CreateLeadCard(this LeadCardsResource resource, int communityId, LeadCardVM request)
            => resource.CreateLeadCardAsync(communityId, request).GetAwaiter().GetResult();
    }
}