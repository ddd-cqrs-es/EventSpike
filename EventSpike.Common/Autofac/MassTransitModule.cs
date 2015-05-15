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

                bus.Subscribe(subscribe => subscribe.LoadFrom(context.ResolveNamed<ILifetimeScope>(MassTransitInstanceNames.LifetimeScope)));
            }))
            .As<IServiceBus>()
            .As<ServiceBus>()
            .SingleInstance();

            builder.RegisterType<MassTransitTenantPublisher>()
                .WithParameter(ResolvedParameter.ForNamed<string>(InstanceNames.CurrentTenantId));
        }

        public static class MassTransitInstanceNames
        {
            public const string
                LifetimeScope = "MassTransitEndpointName",
                DataEndpointName = "MassTransitEndpointName",
                SubscriptionEndpointName = "MassTransitSubscriptionEndpointName";
        }
    }
}