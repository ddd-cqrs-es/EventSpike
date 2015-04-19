namespace NEventStore.Spike.Common
{
    public class ConnectionStringEnvelope
    {
        public ConnectionStringEnvelope(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}