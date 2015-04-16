using System.Collections.Concurrent;
using NEventStore.Spike.Common.Factories;
using StructureMap.Configuration.DSL;

namespace NEventStore.Spike.Common.Registries
{
    public class NEventStoreRegistry :
        Registry
    {
        static readonly private ConcurrentDictionary<string, IStoreEvents> EventStoreCache = new ConcurrentDictionary<string, IStoreEvents>();

        private const string SingleTenantConnectionString = "Database=Approval;Server=(local);Integrated Security=SSPI;";

        public NEventStoreRegistry()
        {
            For<TenantProvider<ConnectionStringEnvelope>>()
                .Use(new TenantProvider<ConnectionStringEnvelope>(tenantId => new ConnectionStringEnvelope(SingleTenantConnectionString)));

            For<TenantProvider<IStoreEvents>>()
                .Use(context => new TenantProvider<IStoreEvents>(tenantId => EventStoreCache.GetOrAdd(tenantId, context.GetInstance<TenantEventStoreFactory>().Construct)))
                .Singleton();
        }
    }
}