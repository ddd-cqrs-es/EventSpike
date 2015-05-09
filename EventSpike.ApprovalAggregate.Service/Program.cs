using System.Configuration;
using EventSpike.ApprovalAggregate.CommonDomain;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
using MassTransit;
using StructureMap;
using Topshelf;

namespace EventSpike.ApprovalAggregate.Service
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
                configure.AddRegistry<CommonDomainRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();

                configure
                    .For<ConnectionStringSettings>()
                    .Use(context => context.GetInstance<SingleTenantConnectionStringFactory>().GetSettings());

                configure
                    .For<string>()
                    .Add(dataEndpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);

                configure
                    .For<IConsumer>()
                    .Singleton()
                    .Add<MassTransitApprovalCommandConsumer>();
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