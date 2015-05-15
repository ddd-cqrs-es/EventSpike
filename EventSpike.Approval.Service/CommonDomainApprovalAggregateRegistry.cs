using EventSpike.Approval.CommonDomain;
using EventSpike.Common.Registries;
using StructureMap.Configuration.DSL;

namespace EventSpike.Approval.Service
{
    class CommonDomainApprovalAggregateRegistry :
        Registry
    {
        public CommonDomainApprovalAggregateRegistry()
        {
            IncludeRegistry<CommonDomainRegistry>();

            ForConcreteType<MassTransitApprovalCommandConsumer>();
        }
    }
}
