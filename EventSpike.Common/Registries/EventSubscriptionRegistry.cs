using System;
using System.Collections.Generic;
using System.Linq;
using Biggy.Core;
using EventSpike.Common.Biggy;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.NEventStore;
using MemBus;
using MemBus.Configurators;
using MemBus.Subscribing;
using NEventStore.Client;
using StructureMap;
using StructureMap.Configuration.DSL;

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

            For<IBus>()
                .Use(context => BusSetup.StartWith<Conservative>()
                    .Apply<FlexibleSubscribeAdapter>(a => a.RegisterMethods("Handle"))
                    .Construct())
                .OnCreation((context, bus) => WireUpMemBus(context, bus));

            For<IObserveCommits>()
                .Use(context => context.GetInstance<EventSubscriptionFactory>().Construct());
        }

        private static void WireUpMemBus(IContext context, IBus bus)
        {
            var handlers = context.GetAllInstances<IEventHandler>();

            foreach (var handler in handlers)
            {
                bus.Subscribe(handler);
            }
        }
    }
}