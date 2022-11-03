using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace RentDynamics.RdClient.Resources.MessageQueue
{
    [PublicAPI]
    public static class MessageQueueResourceExtensions
    {
        public static int EnqueueMessage(this MessageQueueResource resource, int communityId, int clientId, Dictionary<string, string> payload, string messageType, DateTime? scheduledTime)
            => resource.EnqueueMessageAsync(communityId, clientId, payload, messageType, scheduledTime).GetAwaiter().GetResult();
    }
}
