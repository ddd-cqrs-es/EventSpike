using Biggy.Core;
using EventSpike.Common.Biggy;
using EventSpike.Common.EventSubscription;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class BiggyStreamCheckpointRegistry :
        Registry
    {
        public BiggyStreamCheckpointRegistry()
        {
            For<BiggyList<TenantCheckpointTokenDocument>>()
                .Use(() => new StreamCheckpointTokenBiggyListFactory().Construct())
                .Singleton();

            ForConcreteType<TenantScopedBiggyStoreCheckpointTracker>()
                .Configure
                .Ctor<string>().Is(context => context.GetInstance<TenantIdProvider>()());

            For<ITenantListingProvider>()
                .Use<BiggyTenantListingProvider>();

            Forward<TenantScopedBiggyStoreCheckpointTracker, IStoreCheckpointTracker>();
            Forward<TenantScopedBiggyStoreCheckpointTracker, IStoreCheckpointProvider>();
        }
    }
}