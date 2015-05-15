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
            For<IProvideStoreCheckpoints>()
                .Use<ProjacStoreCheckpointProvider>();

            For<IListTenants>()
                .Use<ProjacTenantListingProvider>();

            For<IHandler>()
                .Add<ProjacApprovalProcessorEventHandler>();
            
            For<INeedInitialization>()
                .Add<ProjacInitializer>();
        }
    }
}