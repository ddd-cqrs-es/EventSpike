using System.Configuration;
using System.Linq;

namespace EventSpike.Common.Autofac
{
    public class ConventionTenantSqlConnectionSettingsFactory
    {
        private readonly string _tenantId;
        private readonly string _storeName;

        private const string
            SingleTenantDbName = "EventSpike",
            SingleTenantConnectionString = "Database={0};Server=(local);Integrated Security=SSPI;MultipleActiveResultSets=true;";

        public ConventionTenantSqlConnectionSettingsFactory(string tenantId, string storeName)
        {
            _tenantId = tenantId;
            _storeName = storeName;
        }

        public ConnectionStringSettings GetSettings()
        {
            var dbName = string.Join("_", SingleTenantDbName, _storeName);

            var connectionName = string.Join("_", new [] {SingleTenantDbName, _storeName, _tenantId}.Where(_ => !string.IsNullOrEmpty(_)));
            var connectionString = string.Format(SingleTenantConnectionString, dbName);

            return new ConnectionStringSettings(connectionName, connectionString, "System.Data.SqlClient");
        }
    }
}