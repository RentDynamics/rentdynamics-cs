using System.Collections.Generic;
using System.Threading.Tasks;
using RentDynamics.RdClient.Models;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    public class LeadCardsResource : BaseRentDynamicsResource
    {
        public LeadCardsResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public Task<Dictionary<string, object>> CreateCommunityLeadCardAsync(int communityId, LeadCardRequest request)
        {
            return ApiClient.PostAsync<LeadCardRequest, Dictionary<string, object>>($"/communities/{communityId}/leadCards", request);
        }
    }
}