using EventSpike.Common.EventSubscription;
using MassTransit;
using NEventStore.Client;

namespace EventSpike.NEventStoreMassTransitIntegration
{
    public class EventSubscriptionMassTransitConsumer :
        Consumes<EventStreamUpdated>.Context
    {
        private readonly IObserveCommits _commitObserver;

        public EventSubscriptionMassTransitConsumer(IObserveCommits commitObserver)
        {
            _commitObserver = commitObserver;
        }

        public void Consume(IConsumeContext<EventStreamUpdated> message)
        {
            _commitObserver.PollNow();
        }
    }
}