using System.Collections.Concurrent;
using EventSpike.Common.Registries;
using StructureMap;

namespace EventSpike.Common
{
    public class StructureMapTenantContainerProvider<TValue> :
        ITenantProvider<TValue>
    {
        private readonly IContainer _container;
        private readonly ConcurrentDictionary<string, IContainer> _tenantContainers = new ConcurrentDictionary<string, IContainer>();

        public StructureMapTenantContainerProvider(IContainer container)
        {
            _container = container;
        }

        public TValue Get(string tenantId)
        {
            var container = _tenantContainers.GetOrAdd(tenantId, _ => CreateTenantContainer(tenantId));

            return container
                .GetInstance<TValue>();
        }

        private IContainer CreateTenantContainer(string tenantId)
        {
            var profileProvider = _container.GetInstance<TenantProfileProvider>();

            var nested = profileProvider != TenantProviderConstants.NullProfileProvider
                ? _container.GetNestedContainer(profileProvider(tenantId))
                : _container.GetNestedContainer();

            nested.Configure(configure => configure.For<TenantIdProvider>()
                .Use(new TenantIdProvider(() => tenantId)));

            return nested;
        }
    }
}