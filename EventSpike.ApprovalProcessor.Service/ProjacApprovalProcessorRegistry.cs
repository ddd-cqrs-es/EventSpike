using EventSpike.ApprovalProcessor.Projac;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using StructureMap.Configuration.DSL;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class ProjacApprovalProcessorRegistry :
        Registry
    {
        public ProjacApprovalProcessorRegistry()
        {
            For<IStoreCheckpointProvider>()
                .Use<ProjacStoreCheckpointProvider>();

            For<ITenantListingProvider>()
                .Use<ProjacTenantListingProvider>();

            For<IHandleEvents>()
                .Add<ProjacApprovalProcessorEventHandler>();
            
            For<INeedInitialization>()
                .Add<ProjacInitializer>();
        }
    }
}