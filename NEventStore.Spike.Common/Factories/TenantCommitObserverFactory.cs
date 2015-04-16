using System;
using System.Collections.Generic;
using NEventStore.Client;
using NEventStore.Spike.Common.StreamCheckpointTracker;

namespace NEventStore.Spike.Common.Factories
{
    public class TenantCommitObserverFactory
    {
        private readonly TenantProvider<IStoreEvents> _eventStoreProvider;
        private readonly TenantProvider<ICheckpointTracker> _checkpointTrackerProvider;
        private readonly IEnumerable<IObserver<ICommit>> _commitObservers;

        public TenantCommitObserverFactory(TenantProvider<IStoreEvents> eventStoreProvider, TenantProvider<ICheckpointTracker> checkpointTrackerProvider, IEnumerable<IObserver<ICommit>> commitObservers)
        {
            _eventStoreProvider = eventStoreProvider;
            _checkpointTrackerProvider = checkpointTrackerProvider;
            _commitObservers = commitObservers;
        }

        public IObserveCommits Construct(string tenantId)
        {
            var eventStore = _eventStoreProvider(tenantId);
            var checkpointTracker = _checkpointTrackerProvider(tenantId);

            var pollingClient = new PollingClient(eventStore.Advanced);

            var checkpoint = checkpointTracker.GetLastCheckpoint();

            var subscription = pollingClient.ObserveFrom(checkpoint);

            foreach (var commitObserver in _commitObservers)
            {
                subscription.Subscribe(commitObserver);
            }

            if (checkpoint == NullCheckpointToken.Value)
            {
                subscription.Start();
            }

            return subscription;
        }
    }
}