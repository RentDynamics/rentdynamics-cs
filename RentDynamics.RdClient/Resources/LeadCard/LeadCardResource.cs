using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.LeadCard
{
    [PublicAPI]
    public class LeadCardResource : BaseRentDynamicsResource
    {
        [UsedImplicitly]
        public LeadCardResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public Task<LeadCardVM> CreateLeadCardAsync(int communityId, LeadCardVM request, CancellationToken token = default)
        {
            return ApiClient.PostAsync<LeadCardVM, LeadCardVM>($"/communities/{communityId}/leadCards", request, token);
        }
    }
}