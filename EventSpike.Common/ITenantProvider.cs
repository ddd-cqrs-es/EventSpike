namespace EventSpike.Common
{
    public interface ITenantProvider<out TValue>
    {
        TValue Get(string tenantId);
    }
}