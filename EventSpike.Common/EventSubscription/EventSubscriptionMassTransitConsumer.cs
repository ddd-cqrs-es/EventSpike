using MassTransit;
using NEventStore.Client;

namespace EventSpike.Common.EventSubscription
{
    public class EventSubscriptionMassTransitConsumer :
        Consumes<EventStreamUpdated>.Context
    {
        private readonly ITenantProvider<IObserveCommits> _commitObserverProvider;

        public EventSubscriptionMassTransitConsumer(ITenantProvider<IObserveCommits> commitObserverProvider)
        {
            _commitObserverProvider = commitObserverProvider;
        }

        public void Consume(IConsumeContext<EventStreamUpdated> message)
        {
            _commitObserverProvider.Get(message.Headers[Constants.TenantIdKey]).PollNow();
        }
    }
}