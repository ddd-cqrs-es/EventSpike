using StructureMap.Configuration.DSL;

namespace EventSpike.Common
{
    public class TenantProviderRegistry : Registry
    {
        public TenantProviderRegistry()
        {
            For(typeof (ITenantProvider<>))
                .Use(typeof (StructureMapTenantProvider<>));
        }
    }
}