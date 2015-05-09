using System;
using System.Configuration;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
using MassTransit;
using StructureMap;

namespace EventSpike.EventConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof (Program).ToEndpointName();

            var container = new Container(configure =>
            {
                configure.AddRegistry<TenantProviderRegistry>();
                configure.AddRegistry<EventSubscriptionRegistry>();
                configure.AddRegistry<MassTransitRegistry>();
                configure.AddRegistry<EventSubscriptionRegistry>();
                configure.AddRegistry<BiggyStreamCheckpointRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();
                
                configure.For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);

                configure
                    .For<IObserver<object>>()
                    .Add<ConsoleOutputProjectionCommitObserver>();

                configure
                    .For<INeedInitialization>()
                    .Add<EventSubscriptionInitializer>();

                configure
                    .For<ConnectionStringSettings>()
                    .Use(context => context.GetInstance<SingleTenantConnectionStringFactory>().GetSettings());
            });

            var initializers = container.GetAllInstances<INeedInitialization>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }


            var bus = container.GetInstance<IServiceBus>();

            Console.ReadLine();
        }
    }
}