using System;
using Autofac.Extras.Multitenant;
using Magnum.Extensions;
using MassTransit;

namespace EventSpike.Common
{
    public class MassTransitMessageHeadersTenantIdentificationProvider : ITenantIdentificationStrategy
    {
        public IServiceBus Bus { get; set; }

        public bool TryIdentifyTenant(out object tenantId)
        {
            tenantId = null;
            try
            {
                tenantId = Bus.With(bus => bus.Context().Headers[Constants.TenantIdKey]);
                return tenantId != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}