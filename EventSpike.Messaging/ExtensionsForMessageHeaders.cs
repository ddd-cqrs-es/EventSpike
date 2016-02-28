using System.Collections.Generic;

namespace EventSpike.Messaging
{
    public static class ExtensionsForMessageHeaders
    {
        public static void CopyFrom(this IDictionary<string, object> target, MessageHeaders source)
        {
            target.CopyFrom(source.ToDictionary());
        }
    }
}
