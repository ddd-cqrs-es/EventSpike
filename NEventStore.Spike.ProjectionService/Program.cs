using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Biggy.Core;
using NEventStore.Client;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.Factories;
using NEventStore.Spike.Common.Registries;
using NEventStore.Spike.Common.StreamCheckpointTracker;
using StructureMap;
using Topshelf;

namespace NEventStore.Spike.ProjectionService
{
    class Program
    {
        static readonly private ConcurrentDictionary<string, IObserveCommits> EventStoreSubscriptionCache = new ConcurrentDictionary<string, IObserveCommits>();

        static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry(new MassTransitRegistry(endpointName));
                configure.AddRegistry(new NEventStoreRegistry());

                configure
                    .For<BiggyList<TenantCheckpointTokenDocument>>()
                    .Use(new StreamCheckpointTokenBiggyListFactory().Construct())
                    .Singleton();

                configure
                    .ForConcreteType<EventSubscriptionBootstrapper>()
                    .Configure
                    .Ctor<IEnumerable<string>>()
                    .Is(context => context.GetInstance<BiggyList<TenantCheckpointTokenDocument>>().Select(x => x.TenantId));

                configure
                    .For<TenantProvider<ICheckpointTracker>>()
                    .Use(context => new TenantProvider<ICheckpointTracker>(tenantId => new TenantScopedBiggyCheckpointTracker(tenantId, context.GetInstance<BiggyList<TenantCheckpointTokenDocument>>())));

                configure
                    .ForConcreteType<TenantEventSubscriptionFactory>()
                    .Configure
                    .Singleton();
                
                configure
                    .For<TenantProvider<IObserveCommits>>()
                    .Use(context => new TenantProvider<IObserveCommits>(tenantId => EventStoreSubscriptionCache.GetOrAdd(tenantId, context.GetInstance<TenantEventSubscriptionFactory>().Construct(tenantId))));

                configure
                    .ForConcreteType<ProjectionNotificationMassTransitConsumer>()
                    .Configure
                    .Singleton();

                configure
                    .For<IObserver<ICommit>>()
                    .Add<ConsoleProjection>();
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ProjectionServiceControl>);
            });
        }
    }
}
