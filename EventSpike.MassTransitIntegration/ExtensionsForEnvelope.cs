using System.Linq;
using EventSpike.Messaging;
using MassTransit;

namespace EventSpike.MassTransitIntegration
{
    public static class ExtensionsForEnvelope
    {
        public static Envelope<TMessage> ToEnvelope<TMessage>(this IConsumeContext<TMessage> context) where TMessage : class
        {
            return new Envelope<TMessage>(context.Message, context.Headers.AsEnumerable().ToMessageHeaders());
        }
    }
}
