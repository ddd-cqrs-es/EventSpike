using StructureMap;

namespace NEventStore.Spike.Common
{
    public class StructureMapTenantProvider<TValue> :
        ITenantProvider<TValue>
    {
        private readonly IContainer _container;

        public StructureMapTenantProvider(IContainer container)
        {
            _container = container;
        }

        public TValue Get(string tenantId)
        {
            return _container.GetInstance<TValue>(tenantId);
        }
    }
}