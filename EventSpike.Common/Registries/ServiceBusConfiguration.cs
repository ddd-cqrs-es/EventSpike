using MassTransit.BusConfigurators;

namespace EventSpike.Common.Registries
{
    public delegate void ServiceBusConfiguration(ServiceBusConfigurator configure);
}