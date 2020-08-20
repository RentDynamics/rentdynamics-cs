using System.Threading;
using System.Threading.Tasks;
using RentDynamics.RdClient.Models;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    public class LeadCardsResource : BaseRentDynamicsResource
    {
        public LeadCardsResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public Task<LeadCard> CreateCommunityLeadCardAsync(int communityId, LeadCard request, CancellationToken token = default)
        {
            return ApiClient.PostAsync<LeadCard, LeadCard>($"/communities/{communityId}/leadCards", request, token);
        }
    }
}