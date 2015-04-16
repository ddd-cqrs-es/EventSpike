using System;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using NEventStore.Spike.Common.Factories;
using StructureMap.Configuration.DSL;

namespace NEventStore.Spike.Common.Registries
{
    public class CommonDomainRegistry :
        Registry
    {
        public CommonDomainRegistry(Action<ConflictDetector> configureConflicts)
        {
            For<TenantProvider<IRepository>>()
                .Use(context => new TenantProvider<IRepository>(tenantId => context.GetInstance<TenantRepositoryFactory>().Construct(tenantId)))
                .Singleton();

            For<IDetectConflicts>()
                .Use<ConflictDetector>()
                .OnCreation(conflictDetector => configureConflicts(conflictDetector));

            For<IConstructAggregates>()
                .Use<AggregateFactory>();

            For<IPipelineHook>()
                .Add<MassTransitNotificationPipelineHook>();
        }
    }
}