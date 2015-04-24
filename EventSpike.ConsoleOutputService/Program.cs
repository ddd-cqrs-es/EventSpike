using System;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
using NEventStore;
using StructureMap;
using Topshelf;

namespace EventSpike.ConsoleOutputService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof (Program).ToEndpointName();
            var serviceName = typeof (Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry<TenantProviderRegistry>();
                configure.AddRegistry<EventSubscriptionRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();
                configure.AddRegistry<MassTransitRegistry>();
                configure.AddRegistry<EventSubscriptionRegistry>();

                configure.For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);

                configure
                    .For<IObserver<ICommit>>()
                    .Add<ConsoleOutputProjectionCommitObserver>();
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ConsoleServiceControl>);
            });
        }
    }
}