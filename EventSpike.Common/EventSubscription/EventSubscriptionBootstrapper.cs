using System.Collections.Generic;
using System.Linq;
using NEventStore.Client;

namespace EventSpike.Common.EventSubscription
{
    public class EventSubscriptionInitializer :
        INeedInitialization
    {
        private readonly IProvideForTenant<IEnumerable<IObserveCommits>> _commitObserversFactory;
        private readonly IListTenants _tenantListingProvider;

        public EventSubscriptionInitializer(IProvideForTenant<IEnumerable<IObserveCommits>> commitObserversFactory, IListTenants tenantListingProvider)
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