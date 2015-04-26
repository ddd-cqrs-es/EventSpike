using System.Configuration;
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
        public NEventStoreRegistry()
        {
            For<IStoreEvents>()
                .Use(context => WireUpEventStore(context));
        }

        private static IStoreEvents WireUpEventStore(IContext context)
        {
            var settings = context.GetInstance<ConnectionStringSettings>();

            var pipelineHooks = context.GetAllInstances<IPipelineHook>();

            return Wireup.Init()
                .UsingSqlPersistence(new ConfigurationConnectionFactory(settings.Name, settings.ProviderName, settings.ConnectionString))
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(pipelineHooks)
                .Build();
        }
    }
}