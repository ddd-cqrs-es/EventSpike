namespace EventSpike.Common
{
    public interface IProvideForTenant<out TValue>
    {
        TValue Get(string tenantId);
    }
}