using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using EventSpike.Common.EventSubscription;
using NEventStore;
using NEventStore.Client;

namespace EventSpike.Common.NEventStore
{
    public class EventSubscriptionFactory
    {
        private readonly IProvideStoreCheckpoints _checkpointTracker;
        private readonly IEnumerable<IObserver<object>> _commitObservers;
        private readonly IStoreEvents _eventStore;

        public EventSubscriptionFactory(IStoreEvents eventStore,
            IProvideStoreCheckpoints checkpointTracker,
            IEnumerable<IObserver<object>> commitObservers)
        {
            _eventStore = eventStore;
            _checkpointTracker = checkpointTracker;
            _commitObservers = commitObservers;
        }

        public IObserveCommits Construct()
        {
            var pollingClient = new PollingClient(_eventStore.Advanced);

            var checkpoint = _checkpointTracker.GetCheckpoint();

            var subscription = pollingClient.ObserveFrom(checkpoint);

            var started = DateTime.UtcNow;

            var liveNotification = subscription
                .SkipWhile(commit => commit.CommitStamp < started)
                .Cast<object>()
                .Merge(subscription.Throttle(TimeSpan.FromSeconds(5)).Select(_ => new SubscriptionIsLive()))
                .Take(1)
                .Select(_ => new SubscriptionIsLive());

            var subscriptionWithLiveNotification = subscription
                .Cast<object>()
                .Merge(liveNotification);
            
            foreach (var commitObserver in _commitObservers)
            {
                subscriptionWithLiveNotification.Subscribe(commitObserver);
            }

            if (checkpoint == NullCheckpointToken.Value)
            {
                subscription.Start();
            }

            return subscription;
        }
    }
}