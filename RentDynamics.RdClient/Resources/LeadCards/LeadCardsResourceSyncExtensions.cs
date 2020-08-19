using System.Collections.Generic;
using RentDynamics.RdClient.Models;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    public static class LeadCardsResourceSyncExtensions
    {
        public static Dictionary<string, object> CreateCommunityLeadCard(this LeadCardsResource resource, int communityId, LeadCardRequest request)
            => resource.CreateCommunityLeadCardAsync(communityId, request).GetAwaiter().GetResult();
    }
}