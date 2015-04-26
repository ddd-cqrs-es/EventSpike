using System;
using System.Collections.Generic;
using MassTransit;

namespace EventSpike.Common.MassTransit
{
    public class MassTransitTenantPublisher :
        IPublisher
    {
        private readonly IServiceBus _bus;
        private readonly TenantIdProvider _tenantIdProvider;

        public MassTransitTenantPublisher(IServiceBus bus, TenantIdProvider tenantIdProvider)
        {
            _bus = bus;
            _tenantIdProvider = tenantIdProvider;
        }

        public void Publish(object message)
        {
            Publish(message, headers => { });
        }

        public void Publish(object message, Action<Dictionary<string, string>> setHeaders)
        {
            _bus.Publish(message, (Action<IPublishContext>)(context =>
            {
                context.SetHeader(Constants.TenantIdKey, _tenantIdProvider());

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
