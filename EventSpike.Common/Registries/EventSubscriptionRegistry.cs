using System;
using System.Collections.Generic;
using System.Linq;
using Biggy.Core;
using EventSpike.Common.Biggy;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.NEventStore;
using NEventStore.Client;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace EventSpike.Common.Registries
{
    public class EventSubscriptionRegistry :
        Registry
    {
        public EventSubscriptionRegistry()
        {
            ForConcreteType<EventSubscriptionMassTransitConsumer>()
                .Configure
                .Singleton();

            ForConcreteType<EventSubscriptionInitializer>()
                .Configure
                .Ctor<IEnumerable<string>>()
                .Is(context => context.GetInstance<BiggyList<TenantCheckpointTokenDocument>>().Select(x => x.TenantId));

            For<IObserver<object>>()
                .Add<MemBusPublisherCommitObserver>();
            
            For<IObserveCommits>()
                .Use(context => context.GetInstance<EventSubscriptionFactory>().Construct())
                .LifecycleIs<ContainerLifecycle>();
        }
    }
}