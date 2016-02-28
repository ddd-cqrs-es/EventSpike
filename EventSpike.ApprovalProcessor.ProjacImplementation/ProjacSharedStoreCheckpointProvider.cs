using System.Configuration;
using EventSpike.Checkpointing;
using Paramol.Executors;
using Paramol.SqlClient;

namespace EventSpike.ApprovalProcessor.ProjacImplementation
{
    public class ProjacSharedStoreCheckpointProvider :
        IProvideStoreCheckpoints
    {
        private readonly string _tenantId;
        private readonly SqlCommandExecutor _executer;

        public ProjacSharedStoreCheckpointProvider(ConnectionStringSettings settings, string tenantId)
        {
            _tenantId = tenantId;

            _executer = new SqlCommandExecutor(settings);
        }

        public string GetCheckpoint()
        {
            const string sql = @"SELECT [CheckpointToken] FROM [StoreCheckpoint] WHERE [StoreId] = @P1";

            using (var reader = _executer.ExecuteReader(TSql.QueryStatement(sql, new { P1 = TSql.NVarCharMax(_tenantId) })))
            {
                if (reader.IsClosed) return null;
                return reader.Read() ? reader.GetString(0) : null;
            }
        }
    }
}