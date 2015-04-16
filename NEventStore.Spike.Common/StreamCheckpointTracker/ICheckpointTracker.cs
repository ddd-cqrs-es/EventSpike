namespace NEventStore.Spike.Common.StreamCheckpointTracker
{
    public interface ICheckpointTracker
    {
        string GetLastCheckpoint();
        void UpdateCheckpoint(string checkpointToken);
    }
}