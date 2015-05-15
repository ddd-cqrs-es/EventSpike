using Autofac;
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
                var dataEndpointName = context.ResolveNamed<string>(InstanceNames.DataEndpointName);

                bus.ReceiveFrom(dataEndpointName.AsEndpointUri());

                bus.UseMsmq(msmq =>
                {
                    var subscriptionEndpointName = context.ResolveNamed<string>(InstanceNames.SubscriptionEndpointName);

                    msmq.UseSubscriptionService(subscriptionEndpointName.AsEndpointUri());
                });

                bus.UseControlBus();

                bus.Subscribe(subscribe => subscribe.LoadFrom(context.ResolveNamed<ILifetimeScope>(InstanceNames.LifetimeScope)));
            }))
            .As<IServiceBus>()
            .SingleInstance();
        }

        public static class InstanceNames
        {
            public const string
                LifetimeScope = "MassTransitEndpointName",
                DataEndpointName = "MassTransitEndpointName",
                SubscriptionEndpointName = "MassTransitSubscriptionEndpointName";
        }
    }
}