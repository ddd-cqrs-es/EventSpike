using System.Configuration;
using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.Common.Autofac;
using EventSpike.Common.MassTransit;
using Topshelf;

namespace EventSpike.Approval.Service
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

            builder.RegisterModule<AggregateSourceApprovalAggregateModule>();

            builder.Register(context => context.Resolve<ConventionTenantSqlConnectionSettingsFactory>().GetSettings()).As<ConnectionStringSettings>();
            
            builder.RegisterInstance(endpointName).Named<string>(MassTransitModule.MassTransitInstanceNames.DataEndpointName);

            var container = builder.Build();

            var tenantContainer = container.Resolve<MultitenantContainer>();

            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(tenantContainer.Resolve<ApprovalServiceControl>);
            });
        }
    }
}