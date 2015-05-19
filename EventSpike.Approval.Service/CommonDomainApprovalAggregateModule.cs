using Autofac;
using EventSpike.Approval.CommonDomain;
using EventSpike.Common.Autofac;

namespace EventSpike.Approval.Service
{
    class CommonDomainApprovalAggregateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<CommonDomainModule>();
            builder.RegisterModule<NEventStoreModule>();

            builder.RegisterType<MassTransitApprovalCommandConsumer>().AsSelf();
        }
    }
}