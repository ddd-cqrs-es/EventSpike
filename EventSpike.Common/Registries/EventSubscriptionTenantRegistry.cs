using EventSpike.Common.EventSubscription;
using EventSpike.Common.NEventStore;
using NEventStore.Client;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class EventSubscriptionTenantRegistry :
        Registry
    {
        public EventSubscriptionTenantRegistry()
        {
            For<IStreamCheckpointTracker>()
                .Singleton()
                .Use<TenantScopedBiggyStreamCheckpointTracker>()
                .Ctor<string>().Is(context => context.GetInstance<string>("tenantId"));

            For<IObserveCommits>()
                .Singleton()
                .Use(context => context.GetInstance<EventSubscriptionFactory>().Construct());
        }
    }
}