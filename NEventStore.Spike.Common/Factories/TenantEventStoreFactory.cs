using System.Collections.Generic;
using NEventStore.Persistence.Sql;

namespace NEventStore.Spike.Common.Factories
{
    public class TenantEventStoreFactory
    {
        private readonly IEnumerable<IPipelineHook> _pipelineHooks;
        private readonly TenantProvider<ConnectionStringEnvelope> _connectionStringProvider;

        public TenantEventStoreFactory(IEnumerable<IPipelineHook> pipelineHooks, TenantProvider<ConnectionStringEnvelope> connectionStringProvider)
        {
            _pipelineHooks = pipelineHooks;
            _connectionStringProvider = connectionStringProvider;
        }

        public IStoreEvents Construct(string tenantId)
        {
            var connectionName = string.Format("NEventStore-{0}", tenantId);
            var connectionString = _connectionStringProvider(tenantId).Value;

            return Wireup.Init()
                .UsingSqlPersistence(new ConfigurationConnectionFactory(connectionName, "System.Data.SqlClient", connectionString))
                .InitializeStorageEngine()
                .UsingJsonSerialization()
                .HookIntoPipelineUsing(_pipelineHooks)
                .Build();
        }
    }
}