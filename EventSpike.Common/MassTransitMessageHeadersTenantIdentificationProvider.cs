using System;
using Autofac.Extras.Multitenant;
using MassTransit;

namespace EventSpike.Common
{
    public class MassTransitMessageHeadersTenantIdentificationProvider : ITenantIdentificationStrategy
    {
        // TODO: does this need to be a Func
        private readonly Func<IServiceBus> _busProvider;

        public MassTransitMessageHeadersTenantIdentificationProvider(Func<IServiceBus> busProvider)
        {
            _busProvider = busProvider;
        }

        public bool TryIdentifyTenant(out object tenantId)
        {
            try
            {
                tenantId = _busProvider().Context().Headers[Constants.TenantIdKey];
                return tenantId != null;
            }
            catch(Exception)
            {
                tenantId = null;
                return false;
            }
        }
    }
}