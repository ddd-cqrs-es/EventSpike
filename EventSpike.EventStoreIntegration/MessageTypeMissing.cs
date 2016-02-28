using System;

namespace EventSpike.EventStoreIntegration
{
    public class MessageTypeMissing : Exception
    {
        public MessageTypeMissing(string typeName) : base(string.Format("A matching type could not be found for {0}", typeName))
        {
        }
    }
}