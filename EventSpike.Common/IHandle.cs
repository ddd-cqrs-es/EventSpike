namespace EventSpike.Common
{
    public interface IHandle<in TMessage> where TMessage : class
    {
        void Handle(TMessage message);
    }
}
