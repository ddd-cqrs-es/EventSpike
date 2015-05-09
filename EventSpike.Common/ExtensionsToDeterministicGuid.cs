using System;
using System.Text;

namespace EventSpike.Common
{
    public static class ExtensionsToDeterministicGuid
    {
        public static Guid Create(this DeterministicGuid source, Guid input)
        {
            return source.Create(input.ToByteArray());
        }

        public static Guid Create(this DeterministicGuid source, string input)
        {
            return source.Create(Encoding.UTF8.GetBytes(input));
        }
    }
}