using MassTransit;
using MassTransit.Saga;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.Registries;
using StructureMap;
using Topshelf;

namespace NEventStore.Spike.RuntimeServices
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container(configure =>
            {
                configure.AddRegistry(new MassTransitRegistry(MassTransitRegistry.SubscriptionEndpointName, busConfigure => busConfigure.SetConcurrentConsumerLimit(1)));

                configure
                    .For(typeof (ISagaRepository<>))
                    .Use(typeof (InMemorySagaRepository<>))
                    .Singleton();
            });

            HostFactory.Run(host =>
            {
                host.DependsOnMsmq();
                host.SetDescription(typeof (Program).ToServiceName());

                host.Service(container.GetInstance<RuntimeServicesControl>);
            });
        }
    }
}
