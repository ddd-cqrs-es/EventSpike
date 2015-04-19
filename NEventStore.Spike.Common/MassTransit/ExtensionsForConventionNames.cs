using System;

namespace NEventStore.Spike.Common.MassTransit
{
    public static class ExtensionsForConventionNames
    {
        public static string ToEndpointName(this Type instance)
        {
            var @namespace = instance.Namespace ?? Guid.NewGuid().ToString();

            return @namespace.Replace(".", "_").ToLowerInvariant();
        }

        public static string ToServiceName(this Type instance)
        {
            var @namespace = instance.Namespace ?? Guid.NewGuid().ToString();

            return @namespace;
        }
    }
}