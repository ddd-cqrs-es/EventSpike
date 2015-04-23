using EventSpike.ApprovalProcessor.Automatonymous;
using EventSpike.ApprovalProcessor.CommonDomain;
using EventSpike.Common;
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

            var rootContainer = new Container(root =>
            {
                root.Profile("automatonymous", profile => profile
                    .For<IEventHandler>()
                    .Singleton()
                    .Use<AutomatonymousApprovalProcessEventHandler>());

                root.Profile("commonDomain", profile =>
                {
                    profile
                        .For<IEventHandler>()
                        .Singleton()
                        .Use<CommonDomainApprovalProcessEventHandler>();

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

            var container = rootContainer.GetProfile("automatonymous");

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ApprovalProcessorServiceControl>);
            });
        }
    }
}