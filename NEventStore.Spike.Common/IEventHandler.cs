namespace NEventStore.Spike.Common
{
    public interface IHandle<in T>
    {
        void Handle(T message);
    }
}
