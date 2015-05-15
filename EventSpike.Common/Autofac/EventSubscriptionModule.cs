using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
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
                .AsSelf()
                .WithParameter(ResolvedParameter.ForNamed<IEnumerable<string>>(InstanceNames.AllTenantIds));

            builder.RegisterType<MemBusPublisherCommitObserver>().As<IObserver<object>>();

            builder.Register(context => context.Resolve<EventSubscriptionFactory>().Construct())
                .As<IObserveCommits>()
                .InstancePerTenant();
        }
    }
}