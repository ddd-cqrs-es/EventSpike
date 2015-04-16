namespace NEventStore.Spike.Common.CheckpointTracker
{
    public interface IStreamCheckpointTracker
    {
        string GetLastCheckpoint();
        void UpdateCheckpoint(string checkpointToken);
    }
}