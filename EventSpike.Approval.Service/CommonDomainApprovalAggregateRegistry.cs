using EventSpike.Approval.AggregateSource;
using EventSpike.Common.Registries;
using MassTransit;
using StructureMap.Configuration.DSL;

namespace EventSpike.Approval.Service
{
    class CommonDomainApprovalAggregateRegistry :
        Registry
    {
        public CommonDomainApprovalAggregateRegistry()
        {
            IncludeRegistry<CommonDomainRegistry>();

            For<IConsumer>()
                .Singleton()
                .Add<MassTransitApprovalCommandConsumer>();
        }
    }
}
