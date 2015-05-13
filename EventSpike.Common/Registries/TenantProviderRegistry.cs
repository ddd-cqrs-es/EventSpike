using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class TenantProviderRegistry : Registry
    {
        public TenantProviderRegistry()
        {
            For(typeof (IProvideForTenant<>))
                .Singleton()
                .Use(typeof (StructureMapTenantContainerProvider<>));

            For<TenantIdProvider>()
                .Singleton()
                .Use(TenantProviderConstants.NullTenantIdProvider);
        }
    }
}