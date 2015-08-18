using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.Common.Autofac;
using EventSpike.Common.MassTransit;
using Topshelf;

namespace EventSpike.Approval.ServiceHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var builder = new ContainerBuilder();

            builder.RegisterModule<AggregateSourceApprovalAggregateModule>();

            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<TenantModule>();
            builder.RegisterModule<MassTransitModule>();
            builder.RegisterModule<NEventStoreModule>();
            builder.RegisterModule<SqlConectionSettingsModule>();

            builder.RegisterInstance(endpointName).Named<string>(MassTransitInstanceNames.DataEndpointName);

            builder.RegisterType<ApprovalServiceControl>().AsSelf();

            var container = builder.Build();

            var tenantContainer = container.Resolve<MultitenantContainer>(TypedParameter.From(container));
            
            HostFactory.Run(host =>
            {
                host.SetServiceName(serviceName);
                host.DependsOnMsmq();

                host.Service(tenantContainer.Resolve<ApprovalServiceControl>);
            });
        }
    }
}