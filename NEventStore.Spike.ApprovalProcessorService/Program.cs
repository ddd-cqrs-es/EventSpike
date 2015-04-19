using System;
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

                configure.For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);

                //configure
                //    .For<IObserver<ICommit>>()
                //    .Add<TObserver>();

                // ...
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