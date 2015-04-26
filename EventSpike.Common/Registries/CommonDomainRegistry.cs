using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using EventSpike.Common.CommonDomain;
using EventSpike.Common.EventSubscription;
using Magnum.Reflection;
using NEventStore;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class CommonDomainRegistry :
        Registry
    {
        public CommonDomainRegistry()
        {
            For<IConstructAggregates>()
                .Singleton()
                .Use<AggregateFactory>();

            For<IConstructSagas>()
                .Singleton()
                .Use<SagaFactory>();

            For<IDetectConflicts>()
                .Singleton()
                .Use<ConflictDetector>()
                .OnCreation((context, conflictDetector) => ConfigureConflictDetector(context, conflictDetector));

            For<IPipelineHook>()
                .Add<MassTransitNotificationPipelineHook>();

            For<IRepository>()
                .Use<EventStoreRepository>();

            For<ISagaRepository>()
                .Use<SagaEventStoreRepository>();
        }

        private static void ConfigureConflictDetector(IContext context, ConflictDetector conflictDetector)
        {
            var registrations = context.GetAllInstances(typeof(ConflictDelegate<,>));

            foreach (var registration in registrations)
            {
                conflictDetector.FastInvoke(registration.GetType().GenericTypeArguments, x => x.Register<object, object>(null), registration);
            }
        }
    }
}