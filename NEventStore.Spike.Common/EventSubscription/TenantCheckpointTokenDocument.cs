namespace NEventStore.Spike.Common.EventSubscription
{
    public class TenantCheckpointTokenDocument
    {
        public string TenantId { get; set; }
        public string CheckpointToken { get; set; }
    }
}