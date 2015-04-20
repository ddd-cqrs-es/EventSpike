using System;
using MemBus;
using MemBus.Configurators;
using MemBus.Subscribing;
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
                    .Add<MemBusPublisherCommitObserver>();

                configure
                    .For<IBus>()
                    .Singleton()
                    .Use(context => BusSetup.StartWith<Conservative>()
                        .Apply<FlexibleSubscribeAdapter>(a => a.RegisterMethods("Handle"))
                        .Construct())
                    .OnCreation((context, bus) => WireUpMemBus(context, bus));

                configure
                    .For<IHandler>()
                    .Singleton();

                configure
                    .Scan(scan =>
                    {
                        scan.AssemblyContainingType<Program>();
                        scan.AddAllTypesOf<IHandler>();
                    });
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ApprovalProcessorServiceControl>);
            });
        }

        private static void WireUpMemBus(IContext context, IBus bus)
        {
            var handlers = context.GetAllInstances<IHandler>();

            foreach (var handler in handlers)
            {
                bus.Subscribe(handler);
            }
        }
    }
}