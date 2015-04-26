using System.Configuration;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using Paramol.Executors;
using Paramol.SqlClient;

namespace EventSpike.ApprovalProcessor.Projac
{
    public class ProjacStoreCheckpointProvider :
        IStoreCheckpointProvider
    {
        private readonly TenantIdProvider _tenantIdProvider;
        private readonly SqlCommandExecutor _executer;

        public ProjacStoreCheckpointProvider(ConnectionStringSettings settings, TenantIdProvider tenantIdProvider)
        {
            _tenantIdProvider = tenantIdProvider;

            _executer = new SqlCommandExecutor(settings);
        }

        public string GetCheckpoint()
        {
            const string sql = @"SELECT [CheckpointToken] FROM [StoreCheckpoint] WHERE [StoreId] = @P1";

            using (var reader = _executer.ExecuteReader(TSql.QueryStatement(sql, new { P1 = TSql.NVarCharMax(_tenantIdProvider()) })))
            {
                if (reader.IsClosed) return null;
                return reader.Read() ? reader.GetString(0) : null;
            }
        }
    }
}