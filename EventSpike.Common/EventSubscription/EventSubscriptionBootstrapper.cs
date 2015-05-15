using System.Collections.Generic;
using NEventStore.Client;

namespace EventSpike.Common.EventSubscription
{
    public class EventSubscriptionInitializer :
        INeedInitialization
    {
        private readonly IEnumerable<IObserveCommits> _commitObservers;

        public EventSubscriptionInitializer(IEnumerable<IObserveCommits> commitObservers)
        {
            _commitObservers = commitObservers;
        }

        public void Initialize()
        {
            foreach (var observer in _commitObservers)
            {
                observer.Start();
            }
        }
    }
}