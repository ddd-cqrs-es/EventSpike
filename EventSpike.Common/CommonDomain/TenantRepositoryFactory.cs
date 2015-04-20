using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using NEventStore;

namespace EventSpike.Common.CommonDomain
{
    public class TenantRepositoryFactory
    {
        private readonly IConstructAggregates _aggregateConstructor;
        private readonly IDetectConflicts _conflictDetector;
        private readonly ITenantProvider<IStoreEvents> _eventStoreProvider;

        public TenantRepositoryFactory(ITenantProvider<IStoreEvents> eventStoreProvider,
            IConstructAggregates aggregateConstructor, IDetectConflicts conflictDetector)
        {
            _eventStoreProvider = eventStoreProvider;
            _aggregateConstructor = aggregateConstructor;
            _conflictDetector = conflictDetector;
        }

        public IRepository Construct(string tenantId)
        {
            var eventStore = _eventStoreProvider.Get(tenantId);

            return new EventStoreRepository(eventStore, _aggregateConstructor, _conflictDetector);
        }
    }
}