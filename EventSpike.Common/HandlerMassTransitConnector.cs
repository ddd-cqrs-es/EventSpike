using System.Linq;
using MassTransit;

namespace EventSpike.Common
{
    public class HandlerMassTransitConnector<TMessage> : Consumes<TMessage>.Context
        where TMessage : class
    {
        private readonly IProvideForTenant<IHandle<Envelope<TMessage>>> _handlerProvider;

        public HandlerMassTransitConnector(IProvideForTenant<IHandle<Envelope<TMessage>>> handlerProvider)
        {
            _handlerProvider = handlerProvider;
        }

        public void Consume(IConsumeContext<TMessage> context)
        {
            var headers = context.Headers
                .ToDictionary(keyIs => keyIs.Key, valueIs => valueIs.Value)
                .ToMessageHeaders()
                .ToArray();

            var envelope = new Envelope<TMessage>(context.Message, headers);

            _handlerProvider.Get(context.Headers[Constants.TenantIdKey]).Handle(envelope);
        }
    }
}