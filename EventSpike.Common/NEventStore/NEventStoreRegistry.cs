using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.NEventStore
{
    public class NEventStoreRegistry :
        Registry
    {
        private const string
            TenantConnectionName = "EventSpike-{0}",
            SingleTenantConnectionString = "Database=EventSpike;Server=(local);Integrated Security=SSPI;";

        public NEventStoreRegistry()
        {
            For<ConnectionStringEnvelope>()
                .MissingNamedInstanceIs
                .ConstructedBy(context => new ConnectionStringEnvelope(string.Format(TenantConnectionName, context.RequestedName), SingleTenantConnectionString));

            For<IStoreEvents>()
                .Singleton()
                .MissingNamedInstanceIs
                .ConstructedBy(context => WireUpEventStore(context));
        }

        private static IStoreEvents WireUpEventStore(IContext context)
        {
            var connectionStringEnvelope = context.GetInstance<ConnectionStringEnvelope>(context.RequestedName);
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