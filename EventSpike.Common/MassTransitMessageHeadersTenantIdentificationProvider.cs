using System;
using Autofac.Extras.Multitenant;
using MassTransit;

namespace EventSpike.Common
{
    public class MassTransitMessageHeadersTenantIdentificationProvider : ITenantIdentificationStrategy
    {
        private readonly IServiceBus _bus;
        private readonly MultitenantContainer _container;

        public MassTransitMessageHeadersTenantIdentificationProvider(IServiceBus bus, MultitenantContainer container)
        {
            _bus = bus;
            _container = container;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            var current = _container.Tag;
            if (current != null)
            {
                tenantId = current;
                return true;
            }

            try
            {
                tenantId = _bus.Context().Headers[Constants.TenantIdKey];
                return true;
            }
            catch(Exception)
            {
                tenantId = null;
                return false;
            }
        }
    }
}