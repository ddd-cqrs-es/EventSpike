using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using EventSpike.Common.EventSubscription;
using Magnum.Reflection;
using NEventStore;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.CommonDomain
{
    public class CommonDomainRegistry :
        Registry
    {
        public CommonDomainRegistry()
        {
            For<IRepository>()
                .Singleton()
                .MissingNamedInstanceIs
                .ConstructedBy(context => context.GetInstance<TenantRepositoryFactory>().Construct(context.RequestedName));

            For<IDetectConflicts>()
                .Use<ConflictDetector>()
                .OnCreation((context, conflictDetector) => ConfigureConflictDetector(context, conflictDetector));

            For<IConstructAggregates>()
                .Use<AggregateFactory>();

            For<IPipelineHook>()
                .Add<MassTransitNotificationPipelineHook>();
        }

        private static void ConfigureConflictDetector(IContext context, ConflictDetector conflictDetector)
        {
            var registrations = context.GetAllInstances(typeof (ConflictDelegate<,>));

            foreach (var registration in registrations)
            {
                conflictDetector.FastInvoke(registration.GetType().GenericTypeArguments,
                    x => x.Register<object, object>(null), registration);
            }
        }
    }
}