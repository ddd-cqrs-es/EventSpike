using System;

namespace EventSpike.EventStoreIntegration
{
    public class MessageTypeMissing : Exception
    {
        public MessageTypeMissing(string typeName) : base($"A matching type could not be found for {typeName}")
        {
        }
    }
}