using System;
using MassTransit;
using MassTransit.BusConfigurators;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace NEventStore.Spike.Common.Registries
{
    public class MassTransitRegistry :
        Registry
    {
        public static string SubscriptionEndpointName = "mt_subscriptions";

        public MassTransitRegistry(string endpointName, Action<ServiceBusConfigurator> busConfiguration = null)
        {
            For<IServiceBus>()
                .Singleton()
                .Use(context => GetBus(context, endpointName, busConfiguration));
        }

        public static IServiceBus GetBus(IContext context, string endpointName, Action<ServiceBusConfigurator> busConfiguration)
        {
            return ServiceBusFactory.New(configure =>
            {
                configure.ReceiveFrom(GetMsmqEndpointUri(endpointName));

                configure.UseMsmq(msmq =>
                {
                    if (endpointName != SubscriptionEndpointName)
                    {
                        msmq.UseSubscriptionService(GetMsmqEndpointUri(SubscriptionEndpointName));
                    }

                    msmq.VerifyMsmqConfiguration();
                });

                configure.UseControlBus();

                configure.Subscribe(subscribe => subscribe.LoadFrom(context.GetInstance<IContainer>()));

                if (busConfiguration != null) busConfiguration(configure);
            });
        }

        private static string GetMsmqEndpointUri(string endpointName)
        {
            return string.Format("msmq://localhost/{0}", endpointName);
        }
    }
}