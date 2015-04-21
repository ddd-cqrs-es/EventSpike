using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class NEventStoreTenantRegistry :
        Registry
    {
        private const string
            TenantConnectionName = "EventSpike-{0}",
            SingleTenantConnectionString = "Database=EventSpike;Server=(local);Integrated Security=SSPI;MultipleActiveResultSets=true;";

        public NEventStoreTenantRegistry()
        {
            For<ConnectionStringEnvelope>()
                .Singleton()
                .Use(context => GetConnectionStringEnvelope(context));

            For<IStoreEvents>()
                .Singleton()
                .Use(context => WireUpEventStore(context));
        }

        private static ConnectionStringEnvelope GetConnectionStringEnvelope(IContext context)
        {
            var tenantId = context.GetInstance<string>("tenantId");

            return new ConnectionStringEnvelope(string.Format(TenantConnectionName, tenantId), SingleTenantConnectionString);
        }

        private static IStoreEvents WireUpEventStore(IContext context)
        {
            var connectionStringEnvelope = context.GetInstance<ConnectionStringEnvelope>();
            var pipelineHooks = context.GetAllInstances<IPipelineHook>();

            return Wireup.Init()
                .UsingSqlPersistence(new ConfigurationConnectionFactory(connectionStringEnvelope.ConnectionName, "System.Data.SqlClient", connectionStringEnvelope.ConnectionString))
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(pipelineHooks)
                .Build();
        }
    }
}