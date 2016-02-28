using System.Collections.Generic;

namespace EventSpike.Common
{
    public static class ExtensionsForMessageHeaders
    {
        public static void CopyFrom(this IDictionary<string, object> target, MessageHeaders source)
        {
            target.CopyFrom(source.ToDictionary());
        }
    }
}
