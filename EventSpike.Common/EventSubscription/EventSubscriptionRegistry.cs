using System.Collections.Generic;
using System.Linq;
using Biggy.Core;
using EventSpike.Common.NEventStore;
using NEventStore.Client;
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
                .ConstructedBy(context => context.GetInstance<TenantEventSubscriptionFactory>().Construct(context.RequestedName));
            
            ForConcreteType<TenantEventSubscriptionFactory>()
                .Configure
                .Singleton();

            ForConcreteType<EventSubscriptionMassTransitConsumer>()
                .Configure
                .Singleton();
        }
    }
}