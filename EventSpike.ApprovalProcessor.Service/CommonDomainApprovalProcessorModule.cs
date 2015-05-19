using Autofac;
using EventSpike.ApprovalProcessor.CommonDomain;
using EventSpike.Common;
using EventSpike.Common.Autofac;
using NEventStore;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class CommonDomainApprovalProcessorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonDomainModule>();
            builder.RegisterModule<BiggyStreamCheckpointModule>();
            builder.RegisterModule<SqlConectionSettingsModule>();

            builder.RegisterType<CommonDomainApprovalProcessEventHandler>().As<IHandler>();

            builder.RegisterType<MassTransitCommandPublisherPipelineHook>().As<IPipelineHook>();
        }
    }
}