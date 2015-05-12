using System;
using Magnum.Reflection;
using MassTransit;
using MassTransit.SubscriptionConfigurators;
using MassTransit.Util;
using StructureMap;

namespace EventSpike.Common.MassTransit
{
    public class StructureMapConsumerFactoryConfigurator
    {
        private readonly SubscriptionBusServiceConfigurator _configurator;
        private readonly IContainer _container;

        public StructureMapConsumerFactoryConfigurator(SubscriptionBusServiceConfigurator configurator, IContainer container)
        {
            _container = container;
            _configurator = configurator;
        }

        public void ConfigureConsumer(Type messageType)
        {
            this.FastInvoke(new[] {messageType}, "Configure");
        }

        [UsedImplicitly]
        public void Configure<T>()
            where T : class, IConsumer
        {
            _configurator.Consumer(new StructureMapConsumerFactory<T>(_container));
        }
    }
}