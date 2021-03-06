﻿using System.Configuration;
using System.Linq;

namespace EventSpike.SqlIntegration
{
    public class ConventionTenantSqlConnectionSettingsFactory
    {
        private readonly string _tenantId;

        private const string
            SingleTenantDbName = "EventSpike",
            SingleTenantConnectionString = "Database={0};Server=(local);Integrated Security=SSPI;MultipleActiveResultSets=true;";

        public ConventionTenantSqlConnectionSettingsFactory(string tenantId)
        {
            _tenantId = tenantId;
        }

        public ConnectionStringSettings GetSettings(string storeName = null)
        {
            var dbName = string.Join("_", new[] {SingleTenantDbName, storeName}.Where(_ => !string.IsNullOrWhiteSpace(_)));

            var connectionName = string.Join("_", new [] {SingleTenantDbName, storeName, _tenantId}.Where(_ => !string.IsNullOrEmpty(_)));
            var connectionString = string.Format(SingleTenantConnectionString, dbName);

            return new ConnectionStringSettings(connectionName, connectionString, "System.Data.SqlClient");
        }
    }
}