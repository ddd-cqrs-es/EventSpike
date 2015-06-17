using System.Collections.Generic;
using System.Linq;
using MassTransit;

namespace EventSpike.Common
{
    public static class ExtensionsForEnvelope
    {
        public static MessageHeader[] ToMessageHeaders<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.Select(tuple => new MessageHeader(tuple.Key as string ?? tuple.Key.ToString(), tuple.Value as string ?? tuple.Value.ToString())).ToArray();
        }

        public static IEnumerable<KeyValuePair<string, string>> ToDictionary(this MessageHeaders source)
        {
            return source.Headers
                .ToLookup(keyIs => keyIs.Key, valueIs => valueIs.Value)
                .ToDictionary(keyIs => keyIs.Key, valueIs => valueIs.FirstOrDefault());
        }

        public static Envelope<TMessage> ToEnvelope<TMessage>(this IConsumeContext<TMessage> context) where TMessage : class
        {
            return new Envelope<TMessage>(context.Message, context.Headers.AsEnumerable().ToMessageHeaders());
        }
    }
}
