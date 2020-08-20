using System.Collections.Generic;
using RentDynamics.RdClient.Models;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    public static class LeadCardsResourceSyncExtensions
    {
        public static LeadCard CreateCommunityLeadCard(this LeadCardsResource resource, int communityId, LeadCard request)
            => resource.CreateCommunityLeadCardAsync(communityId, request).GetAwaiter().GetResult();
    }
}