namespace EventSpike.Common.EventSubscription
{
    public interface IStreamCheckpointTracker
    {
        string GetLastCheckpoint();
        void UpdateCheckpoint(string checkpointToken);
    }
}