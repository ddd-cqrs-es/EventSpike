using System.Configuration;
using System.Linq;

namespace EventSpike.Common.Registries
{
    public class SingleTenantConnectionStringFactory
    {
        private readonly TenantIdProvider _tenantIdProvider;

        private const string
            SingleTenantDbName = "EventSpike",
            SingleTenantConnectionString = "Database={0};Server=(local);Integrated Security=SSPI;MultipleActiveResultSets=true;";

        public SingleTenantConnectionStringFactory(TenantIdProvider tenantIdProvider)
        {
            _tenantIdProvider = tenantIdProvider;
        }

        public ConnectionStringSettings GetSettings()
        {
            return GetSettings(null);
        }

        public ConnectionStringSettings GetSettings(string storeName)
        {
            var tenantId = _tenantIdProvider != TenantProviderConstants.NullTenantIdProvider
                ? _tenantIdProvider()
                : "unspecified";

            var dbName = storeName != null
                ? string.Join("_", SingleTenantDbName, storeName)
                : SingleTenantDbName;

            var connectionName = string.Join("_", new [] {SingleTenantDbName, storeName, tenantId}.Where(_ => !string.IsNullOrEmpty(_)));
            var connectionString = string.Format(SingleTenantConnectionString, dbName);

            return new ConnectionStringSettings(connectionName, connectionString, "System.Data.SqlClient");
        }
    }
}