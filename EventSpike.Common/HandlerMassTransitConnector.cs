using System.Linq;
using MassTransit;

namespace EventSpike.Common
{
    public class HandlerMassTransitConnector<TMessage> : Consumes<TMessage>.Context
        where TMessage : class
    {
        private readonly ITenantProvider<IHandle<Envelope<TMessage>>> _handlerProvider;

        public HandlerMassTransitConnector(ITenantProvider<IHandle<Envelope<TMessage>>> handlerProvider)
        {
            _handlerProvider = handlerProvider;
        }

        public void Consume(IConsumeContext<TMessage> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];

            var headers = context.Headers
                .ToDictionary(keyIs => keyIs.Key, valueIs => valueIs.Value)
                .ToMessageHeaders()
                .ToArray();

            var envelope = new Envelope<TMessage>(context.Message, headers);

            _handlerProvider.Get(tenantId).Handle(envelope);
        }
    }
}