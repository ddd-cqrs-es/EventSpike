using System.Collections.Generic;
using System.Linq;
using NEventStore.Client;

namespace EventSpike.Common.EventSubscription
{
    public class EventSubscriptionInitializer :
        INeedInitialization
    {
        private readonly ITenantProvider<IEnumerable<IObserveCommits>> _commitObserversFactory;
        private readonly ITenantListingProvider _tenantListingProvider;

        public EventSubscriptionInitializer(ITenantProvider<IEnumerable<IObserveCommits>> commitObserversFactory, ITenantListingProvider tenantListingProvider)
        {
            _commitObserversFactory = commitObserversFactory;
            _tenantListingProvider = tenantListingProvider;
        }

        public void Initialize()
        {
            foreach (var observer in _tenantListingProvider.GetTenantIds().SelectMany(tenantId => _commitObserversFactory.Get(tenantId)))
            {
                observer.Start();
            }
        }
    }
}