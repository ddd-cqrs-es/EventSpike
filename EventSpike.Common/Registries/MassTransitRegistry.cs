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
            For<ServiceBusConfigurationDelegate>()
                .Add(context => DefaultConfiguration(context));

            For<IServiceBus>()
                .Singleton()
                .Use(context => GetBus(context));

            For<string>()
                .Add("mt_subscriptions")
                .Named(InstanceNames.SubscriptionEndpointName);

            For<IPublisher>()
                .Use<MassTransitTenantPublisher>();
        }

        private static ServiceBusConfigurationDelegate DefaultConfiguration(IContext context)
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

                configure.Subscribe(subscribe => subscribe.LoadFromWithContext(context.GetInstance<IContainer>()));
            };
        }

        private static IServiceBus GetBus(IContext context)
        {
            var bus = ServiceBusFactory.New(configure =>
            {
                var configurations = context.GetAllInstances<ServiceBusConfigurationDelegate>();

                foreach (var configuration in configurations)
                {
                    configuration(configure);
                }
            });

            return bus;
        }

        public static class InstanceNames
        {
            public const string
                DataEndpointName = "MassTransitEndpointName",
                SubscriptionEndpointName = "MassTransitSubscriptionEndpointName";
        }
    }
}