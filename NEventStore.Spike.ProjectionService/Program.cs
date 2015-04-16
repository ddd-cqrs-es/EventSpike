using System;
using MassTransit;
using NEventStore.Client;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.CheckpointTracker;
using NEventStore.Spike.Common.Registries;
using StructureMap;
using Topshelf;

namespace NEventStore.Spike.ProjectionService
{
    class Program
    {
        static void Main(string[] args)
        {
            var @namespace = typeof(Program).Namespace ?? Guid.NewGuid().ToString();

            var endpointName = @namespace.Replace(".", "_").ToLowerInvariant();

            var container = new Container(configure =>
            {
                configure.AddRegistry(new MassTransitRegistry(endpointName));
                configure.AddRegistry<NEventStoreRegistry>();
                configure.AddRegistry(new CommonDomainRegistry(conflcits => { }));

                configure
                    .For<TenantProvider<IStreamCheckpointTracker>>()
                    .Use(new TenantProvider<IStreamCheckpointTracker>(tenantId => new BiggyFileTenantCheckpointTracker(tenantId)));

                configure
                    .For<TenantProvider<IObserveCommits>>()
                    .Use(context => new TenantProvider<IObserveCommits>(tenantId => default(IObserveCommits)));

                configure
                    .For<Func<IConsumer>>()
                    .Add<Func<ProjectionConsumer>>(context => () => context.GetInstance<ProjectionConsumer>());

                configure
                    .ForConcreteType<ProjectionConsumer>()
                    .Configure
                    .Singleton();
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(@namespace);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ProjectionServiceControl>);
            });
        }
    }
}
