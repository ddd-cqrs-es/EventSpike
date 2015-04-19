using MassTransit;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.CommonDomain;
using NEventStore.Spike.Common.MassTransit;
using NEventStore.Spike.Common.NEventStore;
using StructureMap;
using Topshelf;

namespace NEventStore.Spike.ApprovalService
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