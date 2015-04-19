using StructureMap.Configuration.DSL;

namespace NEventStore.Spike.Common
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