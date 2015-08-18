using Autofac;
using Autofac.Core;
using Autofac.Extras.Multitenant;
using EventSpike.ApprovalProcessor.ProjacImplementation;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;

namespace EventSpike.ApprovalProcessor.ServiceHost
{
    internal class ProjacApprovalProcessorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjacSharedStoreCheckpointProvider>()
                .WithParameter(ResolvedParameter.ForNamed<string>(InstanceNames.CurrentTenantId))
                .As<IProvideStoreCheckpoints>();

            builder.RegisterType<ProjacTenantListingProvider>().As<IListTenants>();

            builder.RegisterType<ProjacApprovalProcessorEventHandler>().As<IHandler>();

            builder.RegisterType<ProjacInitializer>().As<INeedInitialization>().InstancePerTenant();
        }
    }
}