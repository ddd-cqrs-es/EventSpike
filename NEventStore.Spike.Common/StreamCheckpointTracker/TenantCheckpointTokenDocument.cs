namespace NEventStore.Spike.Common.StreamCheckpointTracker
{
    public class TenantCheckpointTokenDocument
    {
        public string TenantId { get; set; }
        public string CheckpointToken { get; set; }
    }
}