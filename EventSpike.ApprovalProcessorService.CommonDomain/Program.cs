using System;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
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
                configure.AddRegistry(new TenantProviderRegistry(tenantConfigure =>
                {
                    tenantConfigure.AddRegistry<NEventStoreTenantRegistry>();
                    tenantConfigure.AddRegistry<EventSubscriptionTenantRegistry>();
                    tenantConfigure.AddRegistry<CommonDomainTenantRegistry>();
                }));

                configure.AddRegistry<MassTransitCommonRegistry>();
                configure.AddRegistry<CommonDomainCommonRegistry>();
                configure.AddRegistry<EventSubscriptionCommonRegistry>();

                configure
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitCommonRegistry.InstanceNames.DataEndpointName);
                
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
