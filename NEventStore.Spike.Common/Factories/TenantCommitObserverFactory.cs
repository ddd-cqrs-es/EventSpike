using NEventStore.Client;
using NEventStore.Spike.Common.CheckpointTracker;

namespace NEventStore.Spike.Common.Factories
{
    public class TenantCommitObserverFactory
    {
        private readonly TenantProvider<IStoreEvents> _eventStoreProvider;
        private readonly TenantProvider<IStreamCheckpointTracker> _checkpointTrackerProvider;

        public TenantCommitObserverFactory(TenantProvider<IStoreEvents> eventStoreProvider, TenantProvider<IStreamCheckpointTracker> checkpointTrackerProvider)
        {
            _eventStoreProvider = eventStoreProvider;
            _checkpointTrackerProvider = checkpointTrackerProvider;
        }

        public IObserveCommits Construct(string tenantId)
        {
            var eventStore = _eventStoreProvider(tenantId);
            var checkpointTracker = _checkpointTrackerProvider(tenantId);

            var pollingClient = new PollingClient(eventStore.Advanced);

            return pollingClient.ObserveFrom(checkpointTracker.GetLastCheckpoint());
        }
    }
}