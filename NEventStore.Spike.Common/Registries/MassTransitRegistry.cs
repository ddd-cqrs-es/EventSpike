using System;
using MassTransit;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace NEventStore.Spike.Common.Registries
{
    public class MassTransitRegistry :
        Registry
    {
        public MassTransitRegistry(string endpointName)
        {
            For<IServiceBus>()
                .Use(GetServiceBus(endpointName))
                .OnCreation((context, bus) => RegisterConsumers(bus, context));
        }

        private void RegisterConsumers(IServiceBus bus, IContext context)
        {
            var consumerFactories = context.GetAllInstances<Func<IConsumer>>();

            foreach (var consumerFactory in consumerFactories)
            {
                bus.SubscribeConsumer(consumerFactory);
            }
        }


        private static IServiceBus GetServiceBus(string endpointName)
        {
            return ServiceBusFactory.New(configure =>
            {
                configure.UseMsmq(msmq => msmq.UseMulticastSubscriptionClient());
                configure.ReceiveFrom(string.Format("msmq://localhost/{0}", endpointName));
            });
        }
    }
}