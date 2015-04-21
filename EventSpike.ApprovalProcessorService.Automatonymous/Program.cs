using System;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.MassTransit;
using EventSpike.Common.NEventStore;
using MemBus;
using MemBus.Configurators;
using MemBus.Subscribing;
using NEventStore;
using StructureMap;
using Topshelf;

namespace EventSpike.ApprovalProcessorService.Automatonymous
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
                configure.AddRegistry<MassTransitRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();
                configure.AddRegistry<EventSubscriptionRegistry>();

                configure
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);

                configure
                    .For<IApprovalProcessorRepository>()
                    .Singleton()
                    .MissingNamedInstanceIs.ConstructedBy(context => context.GetInstance<InMemoryApprovalProcessorRepository>());
                
                configure.Scan(scan =>
                {
                    scan.AssemblyContainingType<Program>();
                    scan.AddAllTypesOf<IHandler>();
                });

                configure.For<IHandler>()
                    .Singleton();
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