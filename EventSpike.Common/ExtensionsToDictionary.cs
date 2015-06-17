using System.Collections.Generic;

namespace EventSpike.Common
{
    public static class ExtensionsToDictionary
    {
        public static void CopyFrom<TKey, TValue>(this IDictionary<string, object> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            foreach (var tuple in source)
            {
                var key = tuple.Key as string ?? tuple.Key.ToString();
                target[key] = tuple.Value;
            }
        }

        public static void CopyFrom(this IDictionary<string, object> target, MessageHeaders source)
        {
            CopyFrom(target, source.ToDictionary());
        }
    }
}
