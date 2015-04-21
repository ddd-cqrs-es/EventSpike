using EventSpike.Common;
using EventSpike.Common.MassTransit;
using EventSpike.Common.Registries;
using StructureMap;
using Topshelf;

namespace EventSpike.ApprovalProcessorService.Automatonymous
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry(new TenantProviderRegistry(tenantConfigure =>
                {
                    tenantConfigure.AddRegistry<NEventStoreTenantRegistry>();
                    tenantConfigure.AddRegistry<EventSubscriptionTenantRegistry>();
                }));

                configure.AddRegistry<MassTransitCommonRegistry>();
                configure.AddRegistry<EventSubscriptionCommonRegistry>();

                configure
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitCommonRegistry.InstanceNames.DataEndpointName);
                
                configure.Scan(scan =>
                {
                    scan.AssemblyContainingType<Program>();
                    scan.AddAllTypesOf<IEventHandler>();
                });

                configure
                    .For<IEventHandler>()
                    .Singleton();
            });

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(container.GetInstance<ApprovalProcessorServiceControl>);
            });
        }
    }
}