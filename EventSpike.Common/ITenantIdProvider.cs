namespace EventSpike.Common
{
    public interface ITenantIdProvider
    {
        object TenantId { get; }
    }
}