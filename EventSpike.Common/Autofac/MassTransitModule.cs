using System.Collections.Generic;
using Autofac;
using EventSpike.Common.MassTransit;
using MassTransit;

namespace EventSpike.Common.Autofac
{
    public class MassTransitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TenantPropogationOutboundInterceptor>();

            builder.RegisterInstance("mt_subscriptions").Named<string>(MassTransitInstanceNames.SubscriptionEndpointName);

            builder.Register(context => new ServiceBusConfigurationDelegate(bus =>
            {
                var dataEndpointUri = context.ResolveNamed<string>(MassTransitInstanceNames.DataEndpointName).AsEndpointUri();

                bus.ReceiveFrom(dataEndpointUri);

                bus.UseMsmq(msmq =>
                {
                    var subscriptionEndpointUri = context.ResolveNamed<string>(MassTransitInstanceNames.SubscriptionEndpointName).AsEndpointUri();

                    msmq.UseSubscriptionService(subscriptionEndpointUri);
                });

                bus.UseControlBus();

                var scope = context.ResolveOptionalNamed<ILifetimeScope>(MassTransitInstanceNames.LifetimeScope) ?? context.Resolve<ILifetimeScope>();

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