using EventSpike.ApprovalProcessor.Automatonymous;
using EventSpike.Common;
using EventSpike.Common.Registries;
using StructureMap.Configuration.DSL;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class AutomatonymousApprovalProcessorRegistry :
        Registry
    {
        public AutomatonymousApprovalProcessorRegistry()
        {
            IncludeRegistry<BiggyStreamCheckpointRegistry>();

            For<IHandleEvents>()
                .Add<AutomatonymousApprovalProcessEventHandler>();
        }
    }
}