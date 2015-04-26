using System;

namespace EventSpike.Common.Registries
{
    public class TenantProviderConstants
    {
        public static readonly TenantProfileProvider NullProfileProvider = tenantId => null;

        public static readonly TenantIdProvider NullTenantIdProvider = () =>
        {
            throw new InvalidOperationException("TenantId is undefined");
        };
    }
}