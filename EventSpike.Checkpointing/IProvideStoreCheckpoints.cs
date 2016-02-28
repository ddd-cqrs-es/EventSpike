namespace EventSpike.Checkpointing
{
    public interface IProvideStoreCheckpoints
    {
        string GetCheckpoint();
    }
}