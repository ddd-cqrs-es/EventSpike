using MassTransit;
using NEventStore.Client;
using NEventStore.Spike.Common;

namespace NEventStore.Spike.ProjectionService
{
    public class ProjectionConsumer :
        Consumes<StreamUpdated>.All
    {
        private readonly TenantProvider<IObserveCommits> _commitObserverProvider;

        public ProjectionConsumer(TenantProvider<IObserveCommits> commitObserverProvider)
        {
            _commitObserverProvider = commitObserverProvider;
        }

        public void Consume(StreamUpdated message)
        {
            _commitObserverProvider(message.TenantId).PollNow();
        }
    }
}
