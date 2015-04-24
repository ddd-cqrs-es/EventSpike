using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class NEventStoreRegistry :
        Registry
    {
        private const string
            TenantConnectionName = "EventSpike-{0}",
            SingleTenantConnectionString = "Database=EventSpike;Server=(local);Integrated Security=SSPI;MultipleActiveResultSets=true;";

        public NEventStoreRegistry()
        {
            For<IStoreEvents>()
                .Use(context => WireUpEventStore(context));
        }

        private static IStoreEvents WireUpEventStore(IContext context)
        {
            var tenantId = context.GetInstance<string>(TenantProviderConstants.TenantIdInstanceKey);

            var connectionName = string.Format(TenantConnectionName, tenantId);

            var pipelineHooks = context.GetAllInstances<IPipelineHook>();

            return Wireup.Init()
                .UsingSqlPersistence(new ConfigurationConnectionFactory(connectionName, "System.Data.SqlClient", SingleTenantConnectionString))
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(pipelineHooks)
                .Build();
        }
    }
}