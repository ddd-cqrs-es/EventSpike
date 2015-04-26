namespace EventSpike.Common.EventSubscription
{
    public interface IStoreCheckpointTracker
    {
        void UpdateCheckpoint(string checkpointToken);
    }
}