using EventSpike.ApprovalProcessor.CommonDomain;
using EventSpike.Common;
using EventSpike.Common.Registries;
using NEventStore;
using StructureMap.Configuration.DSL;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class CommonDomainApprovalProcessorRegistry :
        Registry
    {
        public CommonDomainApprovalProcessorRegistry()
        {
            IncludeRegistry<CommonDomainRegistry>();

            IncludeRegistry<BiggyStreamCheckpointRegistry>();

            For<IEventHandler>()
                .Add<CommonDomainApprovalProcessEventHandler>();

            For<IPipelineHook>()
                .Add<MassTransitCommandPublisherPipelineHook>();
        }
    }
}