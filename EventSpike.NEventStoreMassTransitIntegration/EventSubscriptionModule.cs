using System;
using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.NEventStoreIntegration;
using EventSpike.Runtime;

namespace EventSpike.NEventStoreMassTransitIntegration
{
    public class EventSubscriptionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventSubscriptionMassTransitConsumer>()
                .AsSelf()
                .InstancePerTenant();

            builder.RegisterType<EventSubscriptionInitializer>()
                .As<INeedInitialization>()
                .InstancePerTenant();

            builder.RegisterType<MemBusPublisherCommitObserver>()
                .As<IObserver<object>>()
                .InstancePerTenant();
        }
    }
}