using MassTransit;
using NEventStore.Client;

namespace EventSpike.Common.EventSubscription
{
    public class EventSubscriptionMassTransitConsumer :
        Consumes<EventStreamUpdated>.Context
    {
        private readonly IProvideForTenant<IObserveCommits> _commitObserverProvider;

        public EventSubscriptionMassTransitConsumer(IProvideForTenant<IObserveCommits> commitObserverProvider)
        {
            _commitObserverProvider = commitObserverProvider;
        }

        public void Consume(IConsumeContext<EventStreamUpdated> message)
        {
            _commitObserverProvider.Get(message.Headers[Constants.TenantIdKey]).PollNow();
        }
    }
}