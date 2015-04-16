using CommonDomain;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;

namespace NEventStore.Spike.Common.Factories
{
    public class TenantRepositoryFactory
    {
        private readonly TenantProvider<IStoreEvents> _eventStoreProvider;
        private readonly IConstructAggregates _aggregateConstructor;
        private readonly IDetectConflicts _conflictDetector;

        public TenantRepositoryFactory(TenantProvider<IStoreEvents> eventStoreProvider, IConstructAggregates aggregateConstructor, IDetectConflicts conflictDetector)
        {
            _eventStoreProvider = eventStoreProvider;
            _aggregateConstructor = aggregateConstructor;
            _conflictDetector = conflictDetector;
        }

        public IRepository Construct(string tenantId)
        {
            var eventStore = _eventStoreProvider(tenantId);

            return new EventStoreRepository(eventStore, _aggregateConstructor, _conflictDetector);
        }
    }
}