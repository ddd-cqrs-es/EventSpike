using System;

namespace NEventStore.Spike.Common.EventSubscription
{
    public class EventStreamUpdated
    {
        public string StreamId { get; set; }
        public Guid CausationId { get; set; }
        public string TenantId { get; set; }
        public string CheckpointToken { get; set; }
    }
}