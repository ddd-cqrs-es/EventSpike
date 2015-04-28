using System;

namespace EventSpike.Common
{
    public static class ExtensionsToString
    {
        public static Guid ToGuid(this string source)
        {
            return Guid.Parse(source);
        }
    }
}
