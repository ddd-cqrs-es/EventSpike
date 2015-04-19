using System.Collections.Generic;
using NEventStore.Persistence.Sql;
using NEventStore.Persistence.Sql.SqlDialects;

namespace NEventStore.Spike.Common.NEventStore
{
    public class TenantEventStoreFactory
    {
        private readonly ITenantProvider<ConnectionStringEnvelope> _connectionStringProvider;
        private readonly IEnumerable<IPipelineHook> _pipelineHooks;

        public TenantEventStoreFactory(IEnumerable<IPipelineHook> pipelineHooks,
            ITenantProvider<ConnectionStringEnvelope> connectionStringProvider)
        {
            _pipelineHooks = pipelineHooks;
            _connectionStringProvider = connectionStringProvider;
        }

        public IStoreEvents Construct(string tenantId)
        {
            var connectionStringEnvelope = _connectionStringProvider.Get(tenantId);

            return Wireup.Init()
                .UsingSqlPersistence(new ConfigurationConnectionFactory(connectionStringEnvelope.ConnectionName, "System.Data.SqlClient", connectionStringEnvelope.ConnectionString))
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(_pipelineHooks)
                .Build();
        }
    }
}