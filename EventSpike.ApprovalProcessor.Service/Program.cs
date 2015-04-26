using EventSpike.ApprovalProcessor.CommonDomain;
using EventSpike.ApprovalProcessor.Projac;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
using NEventStore;
using StructureMap;
using Topshelf;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry<TenantProviderRegistry>();
                configure.AddRegistry<SingleDbSqlRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();
                configure.AddRegistry<MassTransitRegistry>();
                configure.AddRegistry<EventSubscriptionRegistry>();
                configure.AddRegistry<CommonDomainRegistry>();

                configure
                    .For<IStoreCheckpointProvider>()
                    .Use<ProjacStoreCheckpointProvider>();

                configure
                    .For<ITenantListingProvider>()
                    .Use<ProjacTenantListingProvider>();

                configure
                    .For<IEventHandler>()
                    .Add<ProjacApprovalProcessorEventHandler>();

                configure
                    .For<IPipelineHook>()
                    .Add<CommandPublisherPipelineHook>();

                configure
                    .For<INeedInitialization>()
                    .AddInstances(add =>
                    {
                        add.Type<ProjacInitializer>();
                        add.Type<EventSubscriptionInitializer>();
                    });

                configure
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ApprovalProcessorServiceControl>);
            });
        }
    }
}