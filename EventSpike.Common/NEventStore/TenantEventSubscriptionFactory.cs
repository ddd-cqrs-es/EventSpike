using System;
using System.Collections.Generic;
using EventSpike.Common.EventSubscription;
using NEventStore;
using NEventStore.Client;

namespace EventSpike.Common.NEventStore
{
    public class TenantEventSubscriptionFactory
    {
        private readonly ITenantProvider<IStreamCheckpointTracker> _checkpointTrackerProvider;
        private readonly IEnumerable<IObserver<ICommit>> _commitObservers;
        private readonly ITenantProvider<IStoreEvents> _eventStoreProvider;

        public TenantEventSubscriptionFactory(ITenantProvider<IStoreEvents> eventStoreProvider,
            ITenantProvider<IStreamCheckpointTracker> checkpointTrackerProvider,
            IEnumerable<IObserver<ICommit>> commitObservers)
        {
            _eventStoreProvider = eventStoreProvider;
            _checkpointTrackerProvider = checkpointTrackerProvider;
            _commitObservers = commitObservers;
        }

        public IObserveCommits Construct(string tenantId)
        {
            var eventStore = _eventStoreProvider.Get(tenantId);
            var checkpointTracker = _checkpointTrackerProvider.Get(tenantId);

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