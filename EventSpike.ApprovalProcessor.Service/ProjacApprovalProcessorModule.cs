using Autofac;
using EventSpike.ApprovalProcessor.Projac;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class ProjacApprovalProcessorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjacStoreCheckpointProvider>().As<IProvideStoreCheckpoints>();

            builder.RegisterType<ProjacTenantListingProvider>().As<IListTenants>();

            builder.RegisterType<ProjacApprovalProcessorEventHandler>().As<IHandler>();

            builder.RegisterType<ProjacInitializer>().As<INeedInitialization>();
        }
    }
}