using Autofac;
using EventSpike.Approval.AggregateSource;
using EventSpike.Approval.AggregateSource.Persistence;
using EventSpike.Common;
using EventSpike.Common.Autofac;
using EventSpike.Common.EventSubscription;
using MassTransit;
using NEventStore;

namespace EventSpike.Approval.Service
{
    class AggregateSourceApprovalAggregateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MassTransitApprovalCommandConsumer>().AsSelf();

            builder.RegisterType<MassTransitNotificationPipelineHook>().As<IPipelineHook>();

            builder.RegisterInstance(ApprovalAggregate.Factory);

            builder.RegisterInstance(new ConflictsWithDelegate((uncommitted, committed) => true));

            builder.RegisterInstance(new DetermisticGuidDelegate((guid, input) => new DeterministicGuid(guid).Create(input)));

            builder.RegisterInstance(new ServiceBusConfigurationDelegate(configure => configure.SetConcurrentConsumerLimit(1)));
        }
    }
}