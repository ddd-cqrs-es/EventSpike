namespace EventSpike.Common.EventSubscription
{
    public interface IStreamCheckpointTracker
    {
        void UpdateCheckpoint(string checkpointToken);
    }
}