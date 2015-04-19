namespace NEventStore.Spike.Common
{
    public interface ITenantProvider<out TValue>
    {
        TValue Get(string tenantId);
    }
}