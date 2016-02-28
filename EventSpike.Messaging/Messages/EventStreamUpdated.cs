using System;

namespace EventSpike.Messaging.Messages
{
    public class EventStreamUpdated
    {
        public string StreamId { get; set; }
        public Guid CausationId { get; set; }
        public string CheckpointToken { get; set; }
    }
}