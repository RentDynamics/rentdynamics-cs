using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCards
{
    [PublicAPI]
    public class LeadCardsResource : BaseRentDynamicsResource
    {
        [UsedImplicitly]
        public LeadCardsResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public Task<LeadCard> CreateLeadCardAsync(int communityId, LeadCard request, CancellationToken token = default)
        {
            return ApiClient.PostAsync<LeadCard, LeadCard>($"/communities/{communityId}/leadCards", request, token);
        }
    }
}