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
            var connectionName = string.Format("NEventStore-{0}", tenantId);
            var connectionString = _connectionStringProvider.Get(tenantId).Value;

            return Wireup.Init()
                .UsingSqlPersistence(new ConfigurationConnectionFactory(connectionName, "System.Data.SqlClient",
                    connectionString))
                .WithDialect(new MsSqlDialect())
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(_pipelineHooks)
                .Build();
        }
    }
}