using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using EventSpike.Common.MassTransit;
using MassTransit;

namespace EventSpike.Common.Autofac
{
    public class MassTransitModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance("mt_subscriptions").Named<string>(MassTransitInstanceNames.SubscriptionEndpointName);

            builder.Register(context => new ServiceBusConfigurationDelegate(bus =>
            {
                var dataEndpointName = context.ResolveNamed<string>(MassTransitInstanceNames.DataEndpointName);

                bus.ReceiveFrom(dataEndpointName.AsEndpointUri());

                bus.UseMsmq(msmq =>
                {
                    var subscriptionEndpointName = context.ResolveNamed<string>(MassTransitInstanceNames.SubscriptionEndpointName);

                    msmq.UseSubscriptionService(subscriptionEndpointName.AsEndpointUri());
                });

                bus.UseControlBus();

                var scope = context.ResolveOptionalNamed<ILifetimeScope>(MassTransitInstanceNames.LifetimeScope) ??
                            context.Resolve<ILifetimeScope>();

                bus.Subscribe(subscribe => subscribe.LoadFrom(scope));
            }))
            .As<ServiceBusConfigurationDelegate>();

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

            builder.RegisterType<MassTransitTenantPublisher>()
                .WithParameter(ResolvedParameter.ForNamed<string>(InstanceNames.CurrentTenantId));
        }

        public static class MassTransitInstanceNames
        {
            public const string
                LifetimeScope = "MassTransitLifetimeScope",
                DataEndpointName = "MassTransitEndpointName",
                SubscriptionEndpointName = "MassTransitSubscriptionEndpointName";
        }
    }
}