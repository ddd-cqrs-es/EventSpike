namespace EventSpike.Common
{
    public struct MessageHeader
    {
        public readonly string Key;
        public readonly string Value;

        public MessageHeader(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}