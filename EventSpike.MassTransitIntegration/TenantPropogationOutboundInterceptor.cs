using Autofac.Extras.Multitenant;
using MassTransit;

namespace EventSpike.MassTransitIntegration
{
    internal class TenantPropogationOutboundInterceptor : IOutboundMessageInterceptor
    {
        private readonly ITenantIdentificationStrategy _identificationStrategy;

        public TenantPropogationOutboundInterceptor(ITenantIdentificationStrategy identificationStrategy)
        {
            _identificationStrategy = identificationStrategy;
        }

        public void PreDispatch(ISendContext context)
        {
            if (!string.IsNullOrEmpty(context.Headers[Constants.TenantIdKey])) return;

            object tenantId;
            if (_identificationStrategy.TryIdentifyTenant(out tenantId))
            {
                context.SetHeader(Constants.TenantIdKey, tenantId.ToString());
            }
        }

        public void PostDispatch(ISendContext context)
        {
        }
    }
}