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
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var container = new Container(root =>
            {
                root
                    .For<IEventHandler>()
                    .Singleton()
                    .Add<CommonDomainApprovalProcessEventHandler>();

                root
                    .For<IPipelineHook>()
                    .Singleton()
                    .Add<MassTransitCommandDispatcherPipelineHook>();

                root.AddRegistry<TenantProviderRegistry>();
                root.AddRegistry<NEventStoreRegistry>();
                root.AddRegistry<MassTransitRegistry>();
                root.AddRegistry<EventSubscriptionRegistry>();
                root.AddRegistry<CommonDomainRegistry>();

                root
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