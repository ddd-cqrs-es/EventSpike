using System;
using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.NEventStore;
using NEventStore.Client;

namespace EventSpike.Common.Autofac
{
    public class EventSubscriptionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventSubscriptionMassTransitConsumer>().AsSelf().SingleInstance();

            builder.RegisterType<EventSubscriptionInitializer>()
                .As<INeedInitialization>()
                .InstancePerTenant();

            builder.RegisterType<MemBusPublisherCommitObserver>().As<IObserver<object>>();

            builder.Register(context => context.Resolve<EventSubscriptionFactory>().Construct())
                .As<IObserveCommits>()
                .InstancePerTenant();
        }
    }
}