namespace EventSpike.Common.Registries
{
    public class TenantProviderConstants
    {
        public static readonly TenantProfileProvider NullProfileProvider = tenantId => null;
        public const string TenantIdInstanceKey = "tenantId";
    }
}