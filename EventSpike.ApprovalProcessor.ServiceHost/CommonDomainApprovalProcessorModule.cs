using Autofac;
using CommonDomain;
using EventSpike.ApprovalProcessor.CommonDomainImplementation;
using EventSpike.Checkpointing.Biggy;
using EventSpike.Messaging;
using EventSpike.NEventStoreMassTransitIntegration;
using EventSpike.SqlIntegration;
using NEventStore;

namespace EventSpike.ApprovalProcessor.ServiceHost
{
    internal class CommonDomainApprovalProcessorModule<TProcessor> : Module
        where TProcessor : class, ISaga
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonDomainModule>();
            builder.RegisterModule<BiggyStreamCheckpointModule>();
            builder.RegisterModule<SqlConectionSettingsModule>();

            builder.RegisterType<CommonDomainApprovalProcessEventHandler<TProcessor>>().As<IHandler>();
            builder.RegisterType<MassTransitCommandPublisherPipelineHook>().As<IPipelineHook>();
        }
    }
}