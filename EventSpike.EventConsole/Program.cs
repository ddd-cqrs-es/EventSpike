using System;
using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.Common;
using EventSpike.Common.Autofac;
using EventSpike.Common.MassTransit;
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
            builder.RegisterModule<EventSubscriptionModule>();
            builder.RegisterModule<MemBusModule>();
            builder.RegisterModule<MassTransitModule>();
            builder.RegisterModule<BiggyStreamCheckpointModule>();
            builder.RegisterModule<NEventStoreModule>();
            builder.RegisterModule<SqlConectionSettingsModule>();

            builder.RegisterInstance(endpointName).Named<string>(MassTransitModule.MassTransitInstanceNames.DataEndpointName);

            builder.RegisterType<ConsoleOutputProjectionCommitObserver>().As<IObserver<object>>().InstancePerTenant();

            var container = builder.Build();

            var tenantContainer = container.Resolve<MultitenantContainer>(TypedParameter.From(container));

            tenantContainer.Resolve<ISystemInitializer>().Initialize();

            tenantContainer.Resolve<IServiceBus>();

            Console.ReadLine();
        }
    }
}