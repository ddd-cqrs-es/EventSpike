using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Magnum.Extensions;
using MassTransit;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace EventSpike.Common
{
    public class MemBusMassTransitConnectorConvention : IRegistrationConvention
    {
        private readonly HashSet<Type> _connectedTypes = new HashSet<Type>();

        public void Process(Type type, Registry registry)
        {
            if (!type.Implements<IHandler>()) return;

            var handledMessageTypes = type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => method.Name == "Handle")
                .Select(method => method.GetParameters())
                .Where(parameters => parameters.Length == 1)
                .Select(parameters => parameters[0].ParameterType)
                .Where(messageType => messageType.IsGenericType)
                .Where(messageType => messageType.GetGenericTypeDefinition() == typeof(Envelope<>))
                .Select(envelopeType => envelopeType.GetGenericArguments().First())
                .ToList();

            if (!handledMessageTypes.Any()) return;

            foreach (var handledMessageType in handledMessageTypes.Where(messageType => !_connectedTypes.Contains(messageType)))
            {
                _connectedTypes.Add(handledMessageType);
                registry.For(typeof (IConsumer)).Add(typeof (MemBusMassTransitConnector<>).MakeGenericType(handledMessageType));
            }
        }
    }
}