using System;
using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.Checkpointing.Biggy;
using EventSpike.Logging.Logary;
using EventSpike.MassTransitIntegration;
using EventSpike.NEventStoreIntegration;
using EventSpike.NEventStoreMassTransitIntegration;
using EventSpike.Runtime;
using EventSpike.SqlIntegration;
using Logary;
using MassTransit;

namespace EventSpike.EventConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof (Program).ToEndpointName();

            var builder = new ContainerBuilder();

            builder.RegisterModule<TenantModule>();
            builder.RegisterModule<NEventStoreModule>();
            builder.RegisterModule<EventSubscriptionModule>();
            builder.RegisterModule<MemBusModule>();
            builder.RegisterModule<MassTransitModule>();
            builder.RegisterModule<BiggyStreamCheckpointModule>();
            builder.RegisterModule<SqlConectionSettingsModule>();
            builder.RegisterModule<LoggingModule>();

            builder.RegisterInstance(endpointName).Named<string>(InstanceNames.DataEndpointName);

            builder.RegisterType<ConsoleOutputProjectionCommitObserver>().As<IObserver<object>>().InstancePerTenant();

            var container = builder.Build();

            var tenantContainer = container.Resolve<MultitenantContainer>(TypedParameter.From(container));

            tenantContainer.Resolve<LogManager>();

            tenantContainer.Resolve<ISystemInitializer>().Initialize();

            tenantContainer.Resolve<IServiceBus>();

            Console.ReadLine();
        }
    }
}