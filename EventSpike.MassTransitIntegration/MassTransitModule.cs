using System.Collections.Generic;
using Autofac;
using MassTransit;

namespace EventSpike.MassTransitIntegration
{
    public class MassTransitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TenantIdentifierInboundInterceptor>().As<IInboundMessageInterceptor>();
            builder.RegisterType<TenantPropogationOutboundInterceptor>().As<IOutboundMessageInterceptor>();

            builder.RegisterType<TenantPropogationOutboundInterceptor>();

            builder.RegisterInstance("mt_subscriptions").Named<string>(InstanceNames.SubscriptionEndpointName);

            builder.Register(context => new ServiceBusConfigurationDelegate(bus =>
            {
                var dataEndpointUri = context.ResolveNamed<string>(InstanceNames.DataEndpointName).AsEndpointUri();

                bus.ReceiveFrom(dataEndpointUri);

                bus.UseMsmq(msmq =>
                {
                    var subscriptionEndpointUri = context.ResolveNamed<string>(InstanceNames.SubscriptionEndpointName).AsEndpointUri();

                    msmq.UseSubscriptionService(subscriptionEndpointUri);
                });

                bus.UseControlBus();

                var scope = context.ResolveOptionalNamed<ILifetimeScope>(InstanceNames.LifetimeScope) ?? context.Resolve<ILifetimeScope>();

                bus.Subscribe(subscribe => subscribe.LoadFrom(scope));

                foreach (var interceptor in context.Resolve<IEnumerable<IInboundMessageInterceptor>>())
                {
                    bus.AddInboundInterceptor(interceptor);
                }

                foreach (var interceptor in context.Resolve<IEnumerable<IOutboundMessageInterceptor>>())
                {
                    bus.AddOutboundInterceptor(interceptor);
                }
            }))
            .As<ServiceBusConfigurationDelegate>()
            .SingleInstance();

            builder.Register(context =>
            {
                var configurations = context.Resolve<IEnumerable<ServiceBusConfigurationDelegate>>();

                return ServiceBusFactory.New(bus =>
                {
                    foreach (var configuration in configurations)
                    {
                        configuration(bus);
                    }
                });
            }).As<IServiceBus>()
            .SingleInstance();
        }
    }
}