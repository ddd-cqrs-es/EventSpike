using System.Configuration;
using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.Common.Autofac;
using EventSpike.Common.MassTransit;
using Topshelf;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var builder = new ContainerBuilder();

            builder.RegisterModule<TenantModule>();
            builder.RegisterModule<MassTransitModule>();
            builder.RegisterModule<EventSubscriptionModule>();
            builder.RegisterModule<MemBusModule>();
            builder.RegisterModule<NEventStoreModule>();

            builder.RegisterModule<CommonDomainApprovalProcessorModule>();

            builder.RegisterInstance(endpointName).Named<string>(MassTransitModule.MassTransitInstanceNames.DataEndpointName);
            
            builder.Register(context => context.Resolve<ConventionTenantSqlConnectionSettingsFactory>().GetSettings()).As<ConnectionStringSettings>();

            builder.RegisterType<ApprovalProcessorServiceControl>().AsSelf();

            var container = builder.Build();

            var tenantContainer = container.Resolve<MultitenantContainer>(TypedParameter.From(container));
            
            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(tenantContainer.Resolve<ApprovalProcessorServiceControl>);
            });
        }
    }
}