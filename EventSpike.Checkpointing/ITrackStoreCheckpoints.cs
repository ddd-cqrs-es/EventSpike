namespace EventSpike.Checkpointing
{
    public interface ITrackStoreCheckpoints
    {
        void UpdateCheckpoint(string checkpointToken);
    }
}