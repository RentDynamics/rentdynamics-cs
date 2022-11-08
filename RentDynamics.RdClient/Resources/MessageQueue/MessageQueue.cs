using System;
using System.Collections.Generic;
using System.Text;

namespace RentDynamics.RdClient.Resources.MessageQueue
{
    public class MessageQueue
    {
        public int ID { get; set; }
        public int? CommunityId { get; set; }
        public int? ClientId { get; set; }
        public string Payload { get; set; }
        public string MessageType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? StartedProcessingOn { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
