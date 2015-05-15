using System.Configuration;
using EventSpike.Common;
using EventSpike.Common.Autofac;
using EventSpike.Common.EventSubscription;
using EventSpike.Common.MassTransit;
using StructureMap;
using Topshelf;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var container = new Container(configure =>
            {
                configure.AddRegistry<TenantProviderRegistry>();
                configure.AddRegistry<MassTransitRegistry>();
                configure.AddRegistry<EventSubscriptionRegistry>();
                configure.AddRegistry<MemBusRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();

                //configure.AddRegistry<ProjacApprovalProcessorRegistry>();
                //configure.AddRegistry<AutomatonymousApprovalProcessorRegistry>();

                configure.AddRegistry<CommonDomainApprovalProcessorRegistry>();

                configure
                    .For<ConnectionStringSettings>()
                    .Use(context => context.GetInstance<ConventionTenantSqlConnectionSettingsFactory>().GetSettings());

                configure
                    .For<INeedInitialization>()
                    .Add<EventSubscriptionInitializer>();

                configure
                    .For<string>()
                    .Add(endpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);
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