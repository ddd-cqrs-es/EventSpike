using EventSpike.Common;
using EventSpike.Common.CommonDomain;
using EventSpike.Common.MassTransit;
using EventSpike.Common.NEventStore;
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
                configure.AddRegistry<TenantProviderRegistry>();
                configure.AddRegistry<MassTransitRegistry>();
                configure.AddRegistry<NEventStoreRegistry>();
                configure.AddRegistry<CommonDomainRegistry>();

                configure
                    .For<string>()
                    .Add(dataEndpointName)
                    .Named(MassTransitRegistry.InstanceNames.DataEndpointName);

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