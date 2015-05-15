using System;
using EventSpike.Approval.AggregateSource;
using EventSpike.Approval.AggregateSource.Persistence;
using EventSpike.Common;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.Registries;
using MassTransit;
using NEventStore;
using StructureMap.Configuration.DSL;

namespace EventSpike.Approval.Service
{
    class AggregateSourceApprovalAggregateRegistry :
        Registry
    {
        public AggregateSourceApprovalAggregateRegistry()
        {
            ForConcreteType<MassTransitApprovalCommandConsumer>();
            
            For<IPipelineHook>()
                .Add<MassTransitNotificationPipelineHook>();

            For<Func<ApprovalAggregate>>()
                .Use(ApprovalAggregate.Factory);

            For<ConflictsWithDelegate>()
                .Use(context => new ConflictsWithDelegate((uncommitted, committed) => true));

            For<DetermisticGuidDelegate>()
                .Use(context => new DetermisticGuidDelegate((guid, input) => new DeterministicGuid(guid).Create(input)));

            For<ServiceBusConfigurationDelegate>()
                .Add(new ServiceBusConfigurationDelegate(configure => configure.SetConcurrentConsumerLimit(1)));
        }
    }
}