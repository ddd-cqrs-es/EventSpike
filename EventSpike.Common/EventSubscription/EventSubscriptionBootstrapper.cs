using NEventStore.Client;

namespace EventSpike.Common.EventSubscription
{
    public class EventSubscriptionInitializer :
        INeedInitialization
    {
        private readonly ITenantProvider<IObserveCommits> _commitObserverFactory;
        private readonly ITenantListingProvider _tenantListingProvider;

        public EventSubscriptionInitializer(ITenantProvider<IObserveCommits> commitObserverFactory, ITenantListingProvider tenantListingProvider)
        {
            _commitObserverFactory = commitObserverFactory;
            _tenantListingProvider = tenantListingProvider;
        }

        public void Initialize()
        {
            foreach (var tenantId in _tenantListingProvider.GetTenantIds())
            {
                _commitObserverFactory.Get(tenantId).Start();
            }
        }
    }
}