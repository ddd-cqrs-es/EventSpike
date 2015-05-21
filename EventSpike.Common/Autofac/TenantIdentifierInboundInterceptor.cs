using System;
using MassTransit;

namespace EventSpike.Common.Autofac
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