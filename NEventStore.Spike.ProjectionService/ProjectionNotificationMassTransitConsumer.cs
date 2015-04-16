using MassTransit;
using NEventStore.Client;
using NEventStore.Spike.Common;

namespace NEventStore.Spike.ProjectionService
{
    public class ProjectionNotificationMassTransitConsumer :
        Consumes<EventStreamUpdated>.All
    {
        private readonly TenantProvider<IObserveCommits> _commitObserverProvider;

        public ProjectionNotificationMassTransitConsumer(TenantProvider<IObserveCommits> commitObserverProvider)
        {
            _commitObserverProvider = commitObserverProvider;
        }

        public void Consume(EventStreamUpdated message)
        {
            _commitObserverProvider(message.TenantId).PollNow();
        }
    }
}
