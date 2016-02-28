using System.Collections.Generic;
using System.Configuration;
using NEventStore;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;

namespace EventSpike.NEventStoreIntegration
{
    public class NEventStoreFactory
    {
        private readonly ConnectionStringSettings _settings;
        private readonly IEnumerable<IPipelineHook> _pipelineHooks;

        public NEventStoreFactory(ConnectionStringSettings settings, IEnumerable<IPipelineHook> pipelineHooks)
        {
            _settings = settings;
            _pipelineHooks = pipelineHooks;
        }

        public IStoreEvents Create()
        {
            return Wireup.Init()
                .UsingSqlPersistence(new ConfigurationConnectionFactory(_settings.Name, _settings.ProviderName, _settings.ConnectionString))
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(_pipelineHooks)
                .Build();
        }
    }
}