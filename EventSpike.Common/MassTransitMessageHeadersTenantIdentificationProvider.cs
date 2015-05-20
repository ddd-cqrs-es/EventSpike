using System;
using Autofac.Extras.Multitenant;

namespace EventSpike.Common
{
    public class ExplicitThreadStaticTenantIdentificationProvider : ITenantIdentificationStrategy
    {
        [ThreadStatic] private static object _tenantId;

        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = _tenantId;
            return _tenantId != null;
        }

        public static void IdentifyAs(object tenantId)
        {
            _tenantId = tenantId;
        }
    }
}