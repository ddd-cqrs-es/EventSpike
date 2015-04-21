namespace EventSpike.Common
{
    public class ConnectionStringEnvelope
    {
        public ConnectionStringEnvelope(string connectionName, string connectionString)
        {
            ConnectionName = connectionName;
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
        public string ConnectionName { get; set; }
    }
}