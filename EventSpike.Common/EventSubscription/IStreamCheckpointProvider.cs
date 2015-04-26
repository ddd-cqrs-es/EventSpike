namespace EventSpike.Common.EventSubscription
{
    public interface IStoreCheckpointProvider
    {
        string GetLastCheckpoint();
    }
}