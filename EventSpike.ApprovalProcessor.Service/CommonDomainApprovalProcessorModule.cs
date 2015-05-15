using System.Configuration;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Multitenant;
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

            builder.RegisterType<CommonDomainApprovalProcessEventHandler>().As<IHandler>();

            builder.RegisterType<MassTransitCommandPublisherPipelineHook>().As<IPipelineHook>();

            builder.RegisterType<ConventionTenantSqlConnectionSettingsFactory>()
                .WithParameter(ResolvedParameter.ForNamed<string>(InstanceNames.CurrentTenantId));

            builder.Register(context => context.Resolve<ConventionTenantSqlConnectionSettingsFactory>().GetSettings())
                .InstancePerTenant()
                .AsSelf();

            builder.Register(context => context.Resolve<ConventionTenantSqlConnectionSettingsFactory>().GetSettings("Projections"))
                .InstancePerTenant()
                .Named<ConnectionStringSettings>("Projections");
        }
    }
}