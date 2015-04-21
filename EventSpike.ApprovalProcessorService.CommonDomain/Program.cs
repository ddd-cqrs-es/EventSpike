using System;
using System.Windows.Input;
using EventSpike.Common;
using EventSpike.Common.CommonDomain;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.MassTransit;
using EventSpike.Common.NEventStore;
using NEventStore;
using StructureMap;
using Topshelf;

namespace EventSpike.ApprovalProcessorService.CommonDomain
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry<TenantProviderRegistry>();
                configure.AddRegistry<MassTransitRegistry>();
                configure.AddRegistry<CommonDomainRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();
                configure.AddRegistry<EventSubscriptionRegistry>();

                configure
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);
                
                configure
                    .For<IObserver<ICommit>>()
                    .Add<MemBusPublisherCommitObserver>();

                configure.Scan(scan =>
                {
                    scan.AssemblyContainingType<Program>();
                    scan.AddAllTypesOf<IEventHandler>();
                });

                configure
                    .For<IEventHandler>()
                    .Singleton();

                configure
                    .For<IPipelineHook>()
                    .Singleton()
                    .Add<MassTransitCommandDispatcherPipelineHook>();
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
