using System;

namespace NEventStore.Spike.Common
{
    public class StreamUpdated
    {
        public string StreamId { get; set; }
        public Guid CausationId { get; set; }
        public string TenantId { get; set; }
        public string CheckpointToken { get; set; }
    }
}
