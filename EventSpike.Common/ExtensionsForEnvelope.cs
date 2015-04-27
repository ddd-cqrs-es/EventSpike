using System.Collections.Generic;
using System.Linq;

namespace EventSpike.Common
{
    public static class ExtensionsForEnvelope
    {
        public static MessageHeader[] ToMessageHeaders<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return source.Select(tuple => new MessageHeader(tuple.Key as string ?? tuple.Key.ToString(), tuple.Value as string ?? tuple.Value.ToString())).ToArray();
        }

        public static IDictionary<string, string> ToDictionary(this MessageHeaders source)
        {
            return source.Headers.ToDictionary(keyIs => keyIs.Key, valueIs => valueIs.Value);
        }
    }
}
