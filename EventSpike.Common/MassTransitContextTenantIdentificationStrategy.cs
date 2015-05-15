using System;
using Autofac.Extras.Multitenant;
using MassTransit;

namespace EventSpike.Common
{
    public class MassTransitContextTenantIdentificationStrategy : ITenantIdentificationStrategy
    {
        private readonly IServiceBus _bus;

        public MassTransitContextTenantIdentificationStrategy(IServiceBus bus)
        {
            _bus = bus;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
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