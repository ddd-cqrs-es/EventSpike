using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.Common;
using EventSpike.Common.Autofac;
using EventSpike.Common.EventSubscription;
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

            builder.RegisterModule<TenantModlue>();
            builder.RegisterModule<EventSubscriptionModule>();
            builder.RegisterModule<MemBusModule>();
            builder.RegisterModule<MassTransitModule>();
            builder.RegisterModule<BiggyStreamCheckpointModule>();
            builder.RegisterModule<NEventStoreModule>();

            builder.RegisterInstance(endpointName).Named<string>(MassTransitModule.InstanceNames.DataEndpointName);

            builder.RegisterType<ConsoleOutputProjectionCommitObserver>().As<IObserver<object>>().InstancePerTenant();

            builder.RegisterType<EventSubscriptionInitializer>().As<INeedInitialization>();

            builder.RegisterType<ConventionTenantSqlConnectionSettingsFactory>().AsSelf();
            
            builder.Register(context => context.Resolve<ConventionTenantSqlConnectionSettingsFactory>().GetSettings()).As<ConnectionStringSettings>();

            var container = builder.Build();

            var tenantContainer = container.Resolve<MultitenantContainer>();

            var tenantIds = tenantContainer.ResolveNamed<IEnumerable<string>>(InstanceNames.AllTenantIds).ToList();

            var tenantInitializers = tenantIds
                .Select(tenantId => tenantContainer.GetTenantScope(tenantId).Resolve<IEnumerable<INeedInitialization>>().ToList())
                .SelectMany(initializers => initializers);

            foreach (var initializer in tenantInitializers)
            {
                initializer.Initialize();
            }

            tenantContainer.Resolve<IServiceBus>();

            Console.ReadLine();
        }
    }
}