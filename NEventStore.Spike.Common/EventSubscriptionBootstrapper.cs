using System.Collections.Generic;
using NEventStore.Client;

namespace NEventStore.Spike.Common
{
    public class EventSubscriptionBootstrapper
    {
        private readonly IEnumerable<string> _tenantIds;
        private readonly TenantProvider<IObserveCommits> _commitObserverFactory;

        public EventSubscriptionBootstrapper(TenantProvider<IObserveCommits> commitObserverFactory, IEnumerable<string> tenantIds)
        {
            _tenantIds = tenantIds;
            _commitObserverFactory = commitObserverFactory;
        }

        public void ResumeSubscriptions()
        {
            foreach (var tenantId in _tenantIds)
            {
                _commitObserverFactory(tenantId).Start();
            }
        }
    }
}
