namespace NEventStore.Spike.Common
{
    public delegate TValue TenantProvider<out TValue>(string tenantId);
}