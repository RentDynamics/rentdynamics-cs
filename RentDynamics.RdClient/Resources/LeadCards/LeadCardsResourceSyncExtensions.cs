using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    [PublicAPI]
    public static class LeadCardsResourceSyncExtensions
    {
        public static LeadCard CreateLeadCard(this LeadCardsResource resource, int communityId, LeadCard request)
            => resource.CreateLeadCardAsync(communityId, request).GetAwaiter().GetResult();
    }
}