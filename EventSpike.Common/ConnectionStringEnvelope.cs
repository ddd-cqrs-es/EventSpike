namespace EventSpike.Common
{
    public class ConnectionStringEnvelope
    {
        public ConnectionStringEnvelope(string connectionName, string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; private set; }
        public string ConnectionName { get; set; }
    }
}