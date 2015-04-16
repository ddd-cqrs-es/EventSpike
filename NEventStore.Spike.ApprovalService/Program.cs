using System;
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
            var @namespace = typeof (Program).Namespace ?? Guid.NewGuid().ToString();

            var endpointName = @namespace.Replace(".", "_").ToLowerInvariant();

            var container = new Container(configure =>
            {
                configure.AddRegistry(new MassTransitRegistry(endpointName));
                configure.AddRegistry<NEventStoreRegistry>();
                configure.AddRegistry(new CommonDomainRegistry(conflcits => { }));

                configure
                    .For<Func<ApprovalCommandMassTransitConsumer>>()
                    .Add<Func<ApprovalCommandMassTransitConsumer>>(context => () => context.GetInstance<ApprovalCommandMassTransitConsumer>());

                configure
                    .ForConcreteType<ApprovalCommandMassTransitConsumer>()
                    .Configure
                    .Singleton();
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(@namespace);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ApprovalServiceControl>);
            });
        }
    }
}
