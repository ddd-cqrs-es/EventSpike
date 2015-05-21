using System;
using System.Collections.Generic;
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

        public static void IdentifyAs<TIdentifier>(TIdentifier tenantId, IComparer<TIdentifier> comparer) where TIdentifier : class
        {
            if (comparer.Compare(_tenantId as TIdentifier, tenantId) != 0) _tenantId = tenantId;
        }
    }
}