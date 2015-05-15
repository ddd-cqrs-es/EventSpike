using System.Configuration;
using EventSpike.Common.Autofac;
using EventSpike.Common.MassTransit;
using StructureMap;
using Topshelf;

namespace EventSpike.Approval.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dataEndpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry<TenantProviderRegistry>();
                configure.AddRegistry<MassTransitRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();
                configure.AddRegistry<MemBusRegistry>();

                //configure.AddRegistry<CommonDomainApprovalAggregateRegistry>();
                configure.AddRegistry<AggregateSourceApprovalAggregateRegistry>();

                configure
                    .For<ConnectionStringSettings>()
                    .Use(context => context.GetInstance<ConventionTenantSqlConnectionSettingsFactory>().GetSettings());

                configure
                    .For<string>()
                    .Add(dataEndpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);
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