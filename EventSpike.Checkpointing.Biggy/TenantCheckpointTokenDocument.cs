namespace EventSpike.Checkpointing.Biggy
{
    public class TenantCheckpointTokenDocument
    {
        public string TenantId { get; set; }
        public string CheckpointToken { get; set; }
    }
}