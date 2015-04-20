using MassTransit;
using NEventStore.Client;

namespace EventSpike.Common.EventSubscription
{
    public class EventSubscriptionMassTransitConsumer :
        Consumes<EventStreamUpdated>.All
    {
        private readonly ITenantProvider<IObserveCommits> _commitObserverProvider;

        public EventSubscriptionMassTransitConsumer(ITenantProvider<IObserveCommits> commitObserverProvider)
        {
            _commitObserverProvider = commitObserverProvider;
        }

        public void Consume(EventStreamUpdated message)
        {
            _commitObserverProvider.Get(message.TenantId).PollNow();
        }
    }
}