using System;
using System.Linq;
using MassTransit;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace EventSpike.Common
{
    public class HandlerMassTransitConnectorConvention : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (!type.ImplementsInterfaceTemplate(typeof(IHandle<>))) return;

            var handledMessageTypes = type.GetInterfaces()
                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IHandle<>))
                .Select(handlerInterface => handlerInterface.GetGenericArguments().First())
                .Where(messageType => messageType.GetGenericTypeDefinition() == typeof(Envelope<>))
                .Select(envelopeType => envelopeType.GetGenericArguments().First())
                .ToList();

            if (!handledMessageTypes.Any()) return;

            foreach (var handledMessageType in handledMessageTypes)
            {
                registry.For(typeof(IConsumer)).Add(typeof(HandlerMassTransitConnector<>).MakeGenericType(handledMessageType));
            }
        }
    }
}