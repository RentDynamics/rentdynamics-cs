using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RentDynamics.RdClient.Resources.MessageQueue
{
    public class MessageQueueResource : BaseRentDynamicsResource
    {
        [UsedImplicitly]
        public MessageQueueResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public async Task<int> EnqueueMessageAsync(
            int communityId, 
            int clientId,
            Dictionary<string, string> payload, 
            string messageType, 
            DateTime? scheduledTime)
        { 
            var messageQueue = new MessageQueueVM(communityId, clientId, payload, messageType, scheduledTime);

            var result = await ApiClient.PostAsync<MessageQueueVM, int>("/svc/pm-sync/MessageQueue/Enqueue", messageQueue);
            return result;
        }
    }
}
