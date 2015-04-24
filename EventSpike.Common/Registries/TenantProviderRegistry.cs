using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class TenantProviderRegistry : Registry
    {
        public TenantProviderRegistry()
        {
            For(typeof (ITenantProvider<>))
                .Singleton()
                .Use(typeof (StructureMapTenantContainerProvider<>));

            For<TenantProfileProvider>()
                .Singleton()
                .Use(TenantProviderConstants.NullProfileProvider);
        }
    }
}