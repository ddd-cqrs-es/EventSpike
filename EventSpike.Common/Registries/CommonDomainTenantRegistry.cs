using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class CommonDomainTenantRegistry :
        Registry
    {
        public CommonDomainTenantRegistry()
        {
            For<IRepository>()
                .Singleton()
                .Use<EventStoreRepository>();

            For<ISagaRepository>()
                .Singleton()
                .Use<SagaEventStoreRepository>();
        }
    }
}