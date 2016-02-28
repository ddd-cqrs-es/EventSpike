using System;
using EventSpike.Runtime;
using MassTransit;

namespace EventSpike.MassTransitIntegration
{
    public class TenantIdentifierInboundInterceptor : IInboundMessageInterceptor
    {
        public void PreDispatch(IConsumeContext context)
        {
            ExplicitThreadStaticTenantIdentificationProvider.IdentifyAs(context.Headers[Constants.TenantIdKey], StringComparer.OrdinalIgnoreCase);
        }

        public void PostDispatch(IConsumeContext context)
        {
        }
    }
}