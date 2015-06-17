using Autofac;
using Autofac.Core;
using EventSpike.Approval.Common;
using EventSpike.Approval.CommonDomain;
using EventSpike.Common;
using EventSpike.Common.Autofac;

namespace EventSpike.Approval.Service
{
    class CommonDomainApprovalAggregateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonDomainModule>();
            builder.RegisterModule<NEventStoreModule>();

            builder.RegisterType<ApprovalCommandHandler>()
                .Named<IHandler>("ApprovalCommandHandler");

            builder.RegisterType<MassTransitApprovalCommandHandlerConnector>()
                .WithParameter(ResolvedParameter.ForNamed<IHandler>("ApprovalCommandHandler"))
                .AsSelf();
        }
    }
}