using EventSpike.Common.MassTransit;
using MassTransit;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace EventSpike.Common.Registries
{
    public class MassTransitRegistry :
        Registry
    {
        public MassTransitRegistry()
        {
            For<ServiceBusConfiguration>()
                .Add(context => DefaultConfiguration(context));

            For<IServiceBus>()
                .Singleton()
                .Use(context => GetBus(context));

            For<string>()
                .Add("mt_subscriptions")
                .Named(InstanceNames.SubscriptionEndpointName);
        }

        private static ServiceBusConfiguration DefaultConfiguration(IContext context)
        {
            return configure =>
            {
                var dataEndpointName = context.GetInstance<string>(InstanceNames.DataEndpointName);

                configure.ReceiveFrom(dataEndpointName.AsEndpointUri());

                configure.UseMsmq(msmq =>
                {
                    var subscriptionEndpointName = context.GetInstance<string>(InstanceNames.SubscriptionEndpointName);

                    msmq.UseSubscriptionService(subscriptionEndpointName.AsEndpointUri());
                });

                configure.UseControlBus();

                configure.Subscribe(subscribe => subscribe.LoadFrom(context.GetInstance<IContainer>()));
            };
        }

        public static IServiceBus GetBus(IContext context)
        {
            return ServiceBusFactory.New(configure =>
            {
                var configurations = context.GetAllInstances<ServiceBusConfiguration>();

                foreach (var configuration in configurations)
                {
                    configuration(configure);
                }
            });
        }

        public static class InstanceNames
        {
            public const string
                DataEndpointName = "MassTransitEndpointName",
                SubscriptionEndpointName = "MassTransitSubscriptionEndpointName";
        }
    }
}