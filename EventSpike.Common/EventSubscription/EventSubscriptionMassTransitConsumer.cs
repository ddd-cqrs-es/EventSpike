using MassTransit;
using NEventStore.Client;

namespace EventSpike.Common.EventSubscription
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