using EventSpike.ApprovalProcessor.CommonDomain;
using EventSpike.Common;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
using NEventStore;
using StructureMap;
using StructureMap.Configuration.DSL;
using Topshelf;
using ApprovalProcessEventHandler = EventSpike.ApprovalProcessor.Automatonymous.ApprovalProcessEventHandler;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var rootContainer = new Container(root =>
            {
                root.Profile("automatonymous", profile =>
                {
                    profile
                        .For<IEventHandler>()
                        .Singleton()
                        .Use<Automatonymous.ApprovalProcessEventHandler>();
                });

                root.Profile("commonDomain", profile =>
                {
                    profile
                        .For<IEventHandler>()
                        .Singleton()
                        .Use<CommonDomain.ApprovalProcessEventHandler>();

                    profile
                        .For<IPipelineHook>()
                        .Singleton()
                        .Add<MassTransitCommandDispatcherPipelineHook>();
                });
                
                root.AddRegistry(new TenantProviderRegistry(tenantConfigure =>
                {
                    tenantConfigure.AddRegistry<EventSubscriptionTenantRegistry>();

                    tenantConfigure.AddRegistry<NEventStoreTenantRegistry>();

                    tenantConfigure.AddRegistry<CommonDomainTenantRegistry>();
                }));

                root.AddRegistry<MassTransitCommonRegistry>();
                root.AddRegistry<EventSubscriptionCommonRegistry>();

                root
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitCommonRegistry.InstanceNames.DataEndpointName);
            });

            var profileContainer = rootContainer.GetProfile("automatonymous");

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(profileContainer.GetInstance<ApprovalProcessorServiceControl>);
            });
        }
    }
}