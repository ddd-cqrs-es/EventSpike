using System.Configuration;
using EventSpike.ApprovalProcessor.CommonDomain;
using EventSpike.Common;
using EventSpike.Common.Autofac;
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

            For<IHandler>()
                .Add<CommonDomainApprovalProcessEventHandler>();

            For<IPipelineHook>()
                .Add<MassTransitCommandPublisherPipelineHook>();

            For<ConnectionStringSettings>()
                .Add(context => context.GetInstance<ConventionTenantSqlConnectionSettingsFactory>().GetSettings("Projections"))
                .Named("Projections");
        }
    }
}