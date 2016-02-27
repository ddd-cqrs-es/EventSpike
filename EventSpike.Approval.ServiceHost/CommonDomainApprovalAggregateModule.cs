using Autofac;
using Autofac.Core;
using EventSpike.Approval.CommonDomainImplementation;
using EventSpike.Approval.MassTransitIntegration;
using EventSpike.Common;
using EventSpike.Common.Autofac;

namespace EventSpike.Approval.ServiceHost
{
    class CommonDomainApprovalAggregateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonDomainModule>();
            builder.RegisterModule<NEventStoreModule>();

            builder.RegisterType<ApprovalCommandHandler>()
                .Named<IHandler>("ApprovalCommandHandler");

            builder.RegisterType<MassTransitConsumerAdapter>()
                .WithParameter(ResolvedParameter.ForNamed<IHandler>("ApprovalCommandHandler"))
                .AsSelf();
        }
    }
}