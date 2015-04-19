using System;
using MassTransit;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.Registries;
using StructureMap;
using Topshelf;

namespace NEventStore.Spike.ApprovalService
{
    class Program
    {
        static void Main(string[] args)
        {
            var endpointName = typeof (Program).ToEndpointName();
            var serviceName = typeof (Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry(new MassTransitRegistry(endpointName));
                configure.AddRegistry(new NEventStoreRegistry());
                configure.AddRegistry(new CommonDomainRegistry(conflcits => { }));

                configure
                    .For<IConsumer>()
                    .Add<ApprovalCommandMassTransitConsumer>();

                configure
                    .ForConcreteType<ApprovalCommandMassTransitConsumer>()
                    .Configure
                    .Singleton();
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ApprovalServiceControl>);
            });
        }
    }
}
