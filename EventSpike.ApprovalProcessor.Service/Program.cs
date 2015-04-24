using System.Collections.Concurrent;
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
        private const string AutomatonymousProfileName = "automatonymous";
        private const string CommonDomainProfileName = "commonDomain";

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
                        .Use<CommonDomainApprovalProcessEventHandler>();

                    profile
                        .For<IPipelineHook>()
                        .Add<MassTransitCommandDispatcherPipelineHook>();
                });

                var profileConfiguration = new ConcurrentDictionary<string, string>();
                profileConfiguration.TryAdd("tenant-1", "commonDomain");

                root.AddRegistry<TenantProviderRegistry>();
                root.AddRegistry<NEventStoreRegistry>();
                root.AddRegistry<MassTransitRegistry>();
                root.AddRegistry<EventSubscriptionRegistry>();
                
                root
                    .For<TenantProfileProvider>()
                    .Use(new TenantProfileProvider(tenantId => tenantId == "tenant-1" ? CommonDomainProfileName : AutomatonymousProfileName));

                root
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);
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