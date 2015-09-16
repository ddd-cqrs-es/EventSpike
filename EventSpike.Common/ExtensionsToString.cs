using System;
using System.Globalization;

namespace EventSpike.Common
{
    public static class ExtensionsToString
    {
        public static Guid ToGuid(this string source)
        {
            return Guid.Parse(source);
        }

        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (!char.IsUpper(input[0])) return input;

            var chars = input.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                var hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1])) break;

#if !(DOTNET || PORTABLE)
                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
#else
                chars[i] = char.ToLowerInvariant(chars[i]);
#endif
            }

            return new string(chars);
        }
    }
}
