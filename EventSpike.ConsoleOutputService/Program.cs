using System;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
using MassTransit;
using NEventStore;
using StructureMap;

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

            var subscriptionBootstrapper = container.GetInstance<EventSubscriptionBootstrapper>();
            subscriptionBootstrapper.ResumeSubscriptions();

            var bus = container.GetInstance<IServiceBus>();

            Console.ReadLine();
        }
    }
}