using System;
using MemBus;
using MemBus.Configurators;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.EventSubscription;
using NEventStore.Spike.Common.MassTransit;
using NEventStore.Spike.Common.NEventStore;
using StructureMap;
using Topshelf;

namespace NEventStore.Spike.ApprovalProcessorService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var memBusIocSupport = new MemBusStructureMapAdapter();

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

                configure
                    .For<IObserver<ICommit>>()
                    .Add<ApprovalProcessorCommitObserver>();

                configure
                    .For<IBus>()
                    .Singleton()
                    .Use(context => BusSetup.StartWith<Conservative>()
                        .Apply<IoCSupport>(s => s.SetAdapter(context.GetInstance<MemBusStructureMapAdapter>()).SetHandlerInterface(typeof (IHandle<>)))
                        .Construct());

                configure
                    .For(typeof(IHandle<>))
                    .Singleton();

                configure
                    .Scan(scan => scan.AddAllTypesOf(typeof(IHandle<>)));
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