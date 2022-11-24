using JetBrains.Annotations;
using System.Threading.Tasks;

namespace RentDynamics.RdClient.Resources.MessageQueue
{
    public class MessageQueueResource : BaseRentDynamicsResource
    {
        [UsedImplicitly]
        public MessageQueueResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public Task<int> EnqueueMessageAsync(MessageQueueVM messageQueue)
        { 
            return ApiClient.PostAsync<MessageQueueVM, int>("/svc/pm-sync/MessageQueue/Enqueue", messageQueue);
        }
    }
}
