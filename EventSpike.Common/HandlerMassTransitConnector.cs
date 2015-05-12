using System.Linq;
using MassTransit;

namespace EventSpike.Common
{
    public class HandlerMassTransitConnector<TMessage> : Consumes<TMessage>.Context
        where TMessage : class
    {
        private readonly IHandle<Envelope<TMessage>> _handler;

        public HandlerMassTransitConnector(IHandle<Envelope<TMessage>> handler)
        {
            _handler = handler;
        }

        public void Consume(IConsumeContext<TMessage> context)
        {
            var headers = context.Headers
                .ToDictionary(keyIs => keyIs.Key, valueIs => valueIs.Value)
                .ToMessageHeaders()
                .ToArray();

            var envelope = new Envelope<TMessage>(context.Message, headers);

            _handler.Handle(envelope);
        }
    }
}