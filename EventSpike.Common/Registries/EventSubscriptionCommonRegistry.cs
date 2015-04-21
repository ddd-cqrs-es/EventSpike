using System;
using System.Collections.Generic;
using System.Linq;
using Biggy.Core;
using EventSpike.Common.EventSubscription;
using MemBus;
using MemBus.Configurators;
using MemBus.Subscribing;
using NEventStore;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class EventSubscriptionCommonRegistry :
        Registry
    {
        public EventSubscriptionCommonRegistry()
        {
            For<BiggyList<TenantCheckpointTokenDocument>>()
                .Use(() => new StreamCheckpointTokenBiggyListFactory().Construct())
                .Singleton();

            ForConcreteType<EventSubscriptionMassTransitConsumer>()
                .Configure
                .Singleton();

            ForConcreteType<EventSubscriptionBootstrapper>()
                .Configure
                .Ctor<IEnumerable<string>>()
                .Is(context => context.GetInstance<BiggyList<TenantCheckpointTokenDocument>>().Select(x => x.TenantId));

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