using System;
using System.Collections.Generic;
using EventSpike.Common.EventSubscription;
using NEventStore;
using NEventStore.Client;

namespace EventSpike.Common.NEventStore
{
    public class EventSubscriptionFactory
    {
        private readonly IStoreCheckpointProvider _checkpointTracker;
        private readonly IEnumerable<IObserver<ICommit>> _commitObservers;
        private readonly IStoreEvents _eventStore;

        public EventSubscriptionFactory(IStoreEvents eventStore,
            IStoreCheckpointProvider checkpointTracker,
            IEnumerable<IObserver<ICommit>> commitObservers)
        {
            _eventStore = eventStore;
            _checkpointTracker = checkpointTracker;
            _commitObservers = commitObservers;
        }

        public IObserveCommits Construct()
        {
            var pollingClient = new PollingClient(_eventStore.Advanced);

            var checkpoint = _checkpointTracker.GetLastCheckpoint();

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