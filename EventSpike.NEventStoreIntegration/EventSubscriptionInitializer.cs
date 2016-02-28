using System.Collections.Generic;
using EventSpike.Common;
using NEventStore.Client;

namespace EventSpike.NEventStoreIntegration
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