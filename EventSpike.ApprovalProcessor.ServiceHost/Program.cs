﻿using Autofac;
using Autofac.Extras.Multitenant;
using EventSpike.Common.Autofac;
using EventSpike.Common.MassTransit;
using EventSpike.NEventStoreIntegration;
using EventSpike.NEventStoreMassTransitIntegration;
using Topshelf;

namespace EventSpike.ApprovalProcessor.ServiceHost
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var endpointName = typeof(Program).ToEndpointName();
            var serviceName = typeof(Program).ToServiceName();

            var builder = new ContainerBuilder();

            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<TenantModule>();
            builder.RegisterModule<MassTransitModule>();
            builder.RegisterModule<EventSubscriptionModule>();
            builder.RegisterModule<MemBusModule>();
            builder.RegisterModule<NEventStoreModule>();
            builder.RegisterModule<SqlConectionSettingsModule>();

            builder.RegisterModule<CommonDomainApprovalProcessorModule<CommonDomainImplementation.Automatonymous.ApprovalProcessor>>();

            builder.RegisterInstance(endpointName).Named<string>(MassTransitInstanceNames.DataEndpointName);

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