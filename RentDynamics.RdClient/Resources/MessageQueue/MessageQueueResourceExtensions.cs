using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace RentDynamics.RdClient.Resources.MessageQueue
{
    [PublicAPI]
    public static class MessageQueueResourceExtensions
    {
        public static int CreateEnqueueMessageAsync(this MessageQueueResource resource, MessageQueueVM messageQueueVm)
            => resource.CreateEnqueueMessageAsync(messageQueueVm).GetAwaiter().GetResult();

        public static int EnqueueMessage(this MessageQueueResource resource, int communityId, int clientId, Dictionary<string, string> payload, string messageType, DateTime? scheduledTime)
            => resource.EnqueueMessage(communityId, clientId, payload, messageType, scheduledTime).GetAwaiter().GetResult();
    }
}
