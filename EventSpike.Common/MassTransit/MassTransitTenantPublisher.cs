using System;
using System.Collections.Generic;
using MassTransit;

namespace EventSpike.Common.MassTransit
{
    public class MassTransitTenantPublisher :
        IPublishMessages
    {
        private readonly IServiceBus _bus;
        private readonly string _tenantId;

        public MassTransitTenantPublisher(IServiceBus bus, string tenantId)
        {
            _bus = bus;
            _tenantId = tenantId;
        }

        public void Publish(object message)
        {
            Publish(message, headers => { });
        }

        public void Publish(object message, Action<Dictionary<string, string>> setHeaders)
        {
            _bus.Publish(message, (Action<IPublishContext>)(context =>
            {
                context.SetHeader(Constants.TenantIdKey, _tenantId);

                var headers = new Dictionary<string, string>();
                setHeaders(headers);

                foreach (var tuple in headers)
                {
                    context.SetHeader(tuple.Key, tuple.Value);
                }
            }));
        }
    }
}
