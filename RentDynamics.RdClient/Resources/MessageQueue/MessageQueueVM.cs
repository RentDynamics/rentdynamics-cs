using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace RentDynamics.RdClient.Resources.MessageQueue
{
    public class MessageQueueVM
    {
        public int CommunityID { get; set; }
        public int ClientID { get; set; }
        public Dictionary<string, string> Payload { get; set; }
        public string MessageType { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ScheduledStartTime { get; set; }

        [UsedImplicitly]
        public MessageQueueVM(int communityID, int clientID, Dictionary<string, string> payload, string messageType, DateTime? scheduledStartTime)
        {
            CommunityID = communityID;
            ClientID = clientID;
            Payload = payload;
            MessageType = messageType;
            ScheduledStartTime = scheduledStartTime;
        }
    }
}
