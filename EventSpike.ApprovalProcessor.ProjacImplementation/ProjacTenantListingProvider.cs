using System.Collections.Generic;
using System.Configuration;
using EventSpike.Runtime;
using Paramol.Executors;
using Paramol.SqlClient;

namespace EventSpike.ApprovalProcessor.ProjacImplementation
{
    public class ProjacTenantListingProvider :
        IListTenants
    {
        private readonly SqlCommandExecutor _executer;

        public ProjacTenantListingProvider(ConnectionStringSettings settings)
        {
            _executer = new SqlCommandExecutor(settings);
        }

        public IEnumerable<string> GetTenantIds()
        {
            using (var reader = _executer.ExecuteReader(TSql.QueryStatement(@"SELECT [StoreId] FROM [StoreCheckpoint]")))
            {
                if (reader.IsClosed) yield break;

                while (reader.Read())
                {
                    yield return reader.GetString(0);
                }
            }
        }
    }
}