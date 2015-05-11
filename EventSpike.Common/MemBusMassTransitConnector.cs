using System.Linq;
using MassTransit;
using MemBus;

namespace EventSpike.Common
{
    public class MemBusMassTransitConnector<TMessage> : Consumes<TMessage>.Context
        where TMessage : class
    {
        private readonly ITenantProvider<IBus> _busProvider;

        public MemBusMassTransitConnector(ITenantProvider<IBus> busProvider)
        {
            _busProvider = busProvider;
        }

        public void Consume(IConsumeContext<TMessage> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];

            var headers = context.Headers
                .ToDictionary(keyIs => keyIs.Key, valueIs => valueIs.Value)
                .ToMessageHeaders()
                .ToArray();

            var envelope = new Envelope<TMessage>(context.Message, headers);

            _busProvider.Get(tenantId).Publish(envelope);
        }
    }
}
