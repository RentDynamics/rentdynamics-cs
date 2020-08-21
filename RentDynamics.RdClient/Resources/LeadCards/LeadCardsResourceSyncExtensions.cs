using RentDynamics.RdClient.Models;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    public static class LeadCardsResourceSyncExtensions
    {
        public static LeadCard CreateLeadCard(this LeadCardsResource resource, int communityId, LeadCard request)
            => resource.CreateLeadCardAsync(communityId, request).GetAwaiter().GetResult();
    }
}