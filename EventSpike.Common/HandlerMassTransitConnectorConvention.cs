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

            var typeSets = type.GetInterfaces()
                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IHandle<>))
                .Select(handlerInterface => new
                {
                    HandlerInterface = handlerInterface,
                    EnvelopeType = handlerInterface.GetGenericArguments().First()
                })
                .Where(types => types.EnvelopeType.GetGenericTypeDefinition() == typeof(Envelope<>))
                .Select(types => new
                {
                    types.HandlerInterface,
                    types.EnvelopeType,
                    MessageType = types.EnvelopeType.GetGenericArguments().First()
                })
                .ToList();

            if (!typeSets.Any()) return;
            
            foreach (var types in typeSets)
            {
                registry.For(types.HandlerInterface).Use(type);
                registry.For(typeof(IConsumer)).Add(typeof(HandlerMassTransitConnector<>).MakeGenericType(types.MessageType));
            }
        }
    }
}