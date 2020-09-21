using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCard
{
    [PublicAPI]
    public static class LeadCardsResourceExtensions
    {
        public static LeadCardVM CreateLeadCard(this LeadCardResource resource, int communityId, LeadCardVM request)
            => resource.CreateLeadCardAsync(communityId, request).GetAwaiter().GetResult();
    }
}