using System;
using System.Collections.Generic;
using MassTransit;
using MassTransit.Exceptions;
using MassTransit.Pipeline;
using StructureMap;

namespace EventSpike.Common.MassTransit
{
    public class StructureMapConsumerFactory<T> :
        IConsumerFactory<T>
        where T : class
    {
        readonly IContainer _container;

        public StructureMapConsumerFactory(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<Action<IConsumeContext<TMessage>>> GetConsumer<TMessage>(
            IConsumeContext<TMessage> context, InstanceHandlerSelector<T, TMessage> selector)
            where TMessage : class
        {
            var tenantId = context.Headers[Constants.TenantIdKey];

            using (var nestedContainer = _container.GetNestedContainer())
            {
                nestedContainer
                    .Configure(configure => configure.For<TenantIdProvider>().Use(new TenantIdProvider(() => tenantId)));

                var consumer = nestedContainer.GetInstance<T>();

                if (consumer == null)
                    throw new ConfigurationException(string.Format("Unable to resolve type '{0}' from container: ", typeof(T)));

                foreach (var handler in selector(consumer, context))
                {
                    yield return handler;
                }
            }
        }
    }
}