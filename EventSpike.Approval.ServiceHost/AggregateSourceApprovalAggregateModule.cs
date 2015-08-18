using AggregateSource;
using AggregateSource.NEventStore;
using Autofac;
using Autofac.Core;
using EventSpike.Approval.AggregateSourceImplementation;
using EventSpike.Approval.AggregateSourceImplementation.Persistence;
using EventSpike.Approval.MassTransitIntegration;
using EventSpike.Common;
using EventSpike.Common.Autofac;
using EventSpike.Common.EventSubscription;
using MassTransit;
using NEventStore;

namespace EventSpike.Approval.ServiceHost
{
    class AggregateSourceApprovalAggregateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApprovalCommandHandler>()
                .Named<IHandler>("ApprovalCommandHandler");

            builder.RegisterType<MassTransitApprovalCommandHandlerAdapter>()
                .WithParameter(ResolvedParameter.ForNamed<IHandler>("ApprovalCommandHandler"))
                .AsSelf();

            builder.RegisterType<MassTransitNotificationPipelineHook>().As<IPipelineHook>();

            builder.RegisterType<UnitOfWork>().AsSelf();

            builder.RegisterGeneric(typeof (Repository<>));//.InstancePerMessageScope();

            builder.RegisterType<NEventStoreUnitOfWorkCommitter>().AsSelf();

            builder.RegisterInstance(ApprovalAggregate.Factory);

            builder.RegisterInstance(new ConflictsWithDelegate((uncommitted, committed) => true));

            builder.RegisterInstance(new DetermisticGuidDelegate((guid, input) => new DeterministicGuid(guid).Create(input)));

            builder.RegisterInstance(new ServiceBusConfigurationDelegate(configure => configure.SetConcurrentConsumerLimit(1)));
        }
    }
}