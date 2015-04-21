using EventSpike.Common;
using EventSpike.Common.CommonDomain;
using EventSpike.Common.MassTransit;
using EventSpike.Common.NEventStore;
using EventSpike.Common.Registries;
using MassTransit;
using StructureMap;
using Topshelf;

namespace EventSpike.ApprovalService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dataEndpointName = typeof (Program).ToEndpointName();
            var serviceName = typeof (Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry(new TenantProviderRegistry(tenantConfigure =>
                {
                    tenantConfigure.AddRegistry<NEventStoreTenantRegistry>();
                    tenantConfigure.AddRegistry<EventSubscriptionTenantRegistry>();
                    tenantConfigure.AddRegistry<CommonDomainTenantRegistry>();
                }));

                configure.AddRegistry<MassTransitCommonRegistry>();
                configure.AddRegistry<CommonDomainCommonRegistry>();
                configure.AddRegistry<EventSubscriptionCommonRegistry>();

                configure
                    .For<string>()
                    .Add(dataEndpointName)
                    .Named(MassTransitCommonRegistry.InstanceNames.DataEndpointName);

                configure
                    .For<IConsumer>()
                    .Add<ApprovalCommandMassTransitConsumer>();

                configure
                    .ForConcreteType<ApprovalCommandMassTransitConsumer>()
                    .Configure
                    .Singleton();
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ApprovalServiceControl>);
            });
        }
    }
}