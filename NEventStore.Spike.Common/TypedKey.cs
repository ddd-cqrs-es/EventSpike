using System.Collections.Generic;

namespace NEventStore.Spike.Common
{
    // Based from Magnum.Context.TypedKey, but uses Type.Name instead of Type.FullName, and has extensions against IDictionary<TKey, TValue> instead of just non-generic IDictionary
    public class TypedKey<T>
    {
        public bool Equals(TypedKey<T> obj)
        {
            return !ReferenceEquals(null, obj);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(TypedKey<T>)) return false;
            return Equals((TypedKey<T>)obj);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public static string UniqueKey = typeof(TypedKey<T>).Name;
    }

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
