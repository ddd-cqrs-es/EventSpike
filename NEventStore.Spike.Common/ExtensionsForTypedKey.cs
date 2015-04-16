using System.Collections.Generic;
using Magnum.Context;

namespace NEventStore.Spike.Common
{
    public static class ExtensionsForTypedKey
    {
        public static void Store<T>(this IDictionary<string, object> items, T value)
        {
            items[TypedKey<T>.UniqueKey] = value;
        }

        public static void Remove<T>(this IDictionary<string, object> items)
        {
            if (items.Exists<T>())
                items.Remove(TypedKey<T>.UniqueKey);
        }

        public static bool Exists<T>(this IDictionary<string, object> items)
        {
            return items.ContainsKey(TypedKey<T>.UniqueKey);
        }

        public static T Retrieve<T>(this IDictionary<string, object> items)
        {
            return (T)items[TypedKey<T>.UniqueKey];
        }
    }
}
