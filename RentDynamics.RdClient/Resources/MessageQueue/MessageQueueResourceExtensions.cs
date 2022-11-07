using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace RentDynamics.RdClient.Resources.MessageQueue
{
    [PublicAPI]
    public static class MessageQueueResourceExtensions
    {
        public static int EnqueueMessage(this MessageQueueResource resource, MessageQueueVM messageQueue)
            => resource.EnqueueMessageAsync(messageQueue).GetAwaiter().GetResult();
    }
}
