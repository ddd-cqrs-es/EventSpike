using StructureMap;

namespace EventSpike.Common
{
    public class StructureMapTenantContainerProvider<TValue> :
        ITenantProvider<TValue>
    {
        private readonly IContainer _container;

        public StructureMapTenantContainerProvider(IContainer container)
        {
            _container = container;
        }

        public TValue Get(string tenantId)
        {
            return _container.With(new TenantIdProvider(() => tenantId)).GetInstance<TValue>();
        }
    }
}