using System.Collections.Generic;
using NEventStore.Client;

namespace NEventStore.Spike.Common.EventSubscription
{
    public class EventSubscriptionBootstrapper
    {
        private readonly ITenantProvider<IObserveCommits> _commitObserverFactory;
        private readonly IEnumerable<string> _tenantIds;

        public EventSubscriptionBootstrapper(ITenantProvider<IObserveCommits> commitObserverFactory,
            IEnumerable<string> tenantIds)
        {
            _tenantIds = tenantIds;
            _commitObserverFactory = commitObserverFactory;
        }

        public void ResumeSubscriptions()
        {
            foreach (var tenantId in _tenantIds)
            {
                _commitObserverFactory.Get(tenantId).Start();
            }
        }
    }
}