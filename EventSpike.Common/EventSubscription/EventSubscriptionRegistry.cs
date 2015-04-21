using System;
using System.Collections.Generic;
using System.Linq;
using Biggy.Core;
using EventSpike.Common.NEventStore;
using MemBus;
using MemBus.Configurators;
using MemBus.Subscribing;
using NEventStore;
using NEventStore.Client;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.EventSubscription
{
    public class EventSubscriptionRegistry :
        Registry
    {
        public EventSubscriptionRegistry()
        {
            For<BiggyList<TenantCheckpointTokenDocument>>()
                .Use(() => new StreamCheckpointTokenBiggyListFactory().Construct())
                .Singleton();

            ForConcreteType<EventSubscriptionBootstrapper>()
                .Configure
                .Ctor<IEnumerable<string>>()
                .Is(context => context.GetInstance<BiggyList<TenantCheckpointTokenDocument>>().Select(x => x.TenantId));

            For<IStreamCheckpointTracker>()
                .Singleton()
                .MissingNamedInstanceIs
                .ConstructedBy(context => new TenantScopedBiggyStreamCheckpointTracker(context.RequestedName, context.GetInstance<BiggyList<TenantCheckpointTokenDocument>>()));

            For<IObserveCommits>()
                .Singleton()
                .MissingNamedInstanceIs
                .ConstructedBy(context => context.GetInstance<EventSubscriptionFactory>().Construct());

            ForConcreteType<EventSubscriptionFactory>()
                .Configure
                .Ctor<IStoreEvents>().Is(context => context.GetInstance<IStoreEvents>(context.RequestedName))
                .Ctor<IStreamCheckpointTracker>().Is(context => context.GetInstance<IStreamCheckpointTracker>(context.RequestedName));

            ForConcreteType<EventSubscriptionMassTransitConsumer>()
                .Configure
                .Singleton();

            For<IObserver<ICommit>>()
                .Singleton()
                .Add<MemBusPublisherCommitObserver>();

            For<IBus>()
                .Singleton()
                .Use(context => BusSetup.StartWith<Conservative>()
                    .Apply<FlexibleSubscribeAdapter>(a => a.RegisterMethods("Handle"))
                    .Construct())
                .OnCreation((context, bus) => WireUpMemBus(context, bus));
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