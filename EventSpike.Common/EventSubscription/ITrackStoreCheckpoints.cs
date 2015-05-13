namespace EventSpike.Common.EventSubscription
{
    public interface ITrackStoreCheckpoints
    {
        void UpdateCheckpoint(string checkpointToken);
    }
}