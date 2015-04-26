using System.Configuration;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class SingleDbSqlRegistry :
        Registry
    {
        private const string
            TenantConnectionName = "EventSpike-{0}",
            SingleTenantConnectionString = "Database=EventSpike;Server=(local);Integrated Security=SSPI;MultipleActiveResultSets=true;";

        public SingleDbSqlRegistry()
        {
            For<ConnectionStringSettings>()
                .Use(context => GetConnectionStringSettings(context));
        }

        private ConnectionStringSettings GetConnectionStringSettings(IContext context)
        {
            var tenantIdProvider = context.GetInstance<TenantIdProvider>();

            var tenantId = tenantIdProvider != TenantProviderConstants.NullTenantIdProvider
                ? tenantIdProvider()
                : "unspecified";

            var connectionName = string.Format(TenantConnectionName, tenantId);

            return new ConnectionStringSettings(connectionName, SingleTenantConnectionString, "System.Data.SqlClient");
        }
    }
}
