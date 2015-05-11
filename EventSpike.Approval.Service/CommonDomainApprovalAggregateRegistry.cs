using EventSpike.Approval.CommonDomain;
using EventSpike.Common;
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

            Scan(scan =>
            {
                scan.AssemblyContainingType<ApprovalAggregate>();

                scan.AddAllTypesOf(typeof(IHandle<>));

                scan.With(new HandlerMassTransitConnectorConvention());
            });
        }
    }
}
