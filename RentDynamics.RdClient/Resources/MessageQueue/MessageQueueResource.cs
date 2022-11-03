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

        public async Task<int> EnqueueMessage(
            int communityId, 
            int clientId,
            Dictionary<string, string> payload, 
            string messageType, 
            DateTime? scheduledTime)
        { 
            var messageQueue = new MessageQueueVM(communityId, clientId, payload, messageType, scheduledTime);

            var result = await CreateEnqueueMessageAsync(messageQueue);
            return result;
        }

        public Task<int> CreateEnqueueMessageAsync(MessageQueueVM request, CancellationToken token = default)
        {
            return ApiClient.PostAsync<MessageQueueVM, int>("/svc/pm-sync/MessageQueue/Enqueue", request, token);
        }
    }
}
