using System;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.MassTransit;
using EventSpike.Common.NEventStore;
using NEventStore;
using StructureMap;

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
                    scan.AddAllTypesOf<IHandler>();
                });

                configure.For<IHandler>()
                    .Singleton();
            });
        }
    }
}
