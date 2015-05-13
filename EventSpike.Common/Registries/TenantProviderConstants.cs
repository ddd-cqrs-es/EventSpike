using System;

namespace EventSpike.Common.Registries
{
    public class TenantProviderConstants
    {
        public static readonly TenantIdProvider NullTenantIdProvider = () =>
        {
            throw new InvalidOperationException("TenantId is undefined");
        };
    }
}