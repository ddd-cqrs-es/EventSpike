namespace EventSpike.Common.EventSubscription
{
    public interface IProvideStoreCheckpoints
    {
        string GetCheckpoint();
    }
}