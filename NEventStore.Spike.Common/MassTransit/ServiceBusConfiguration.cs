using MassTransit.BusConfigurators;

namespace NEventStore.Spike.Common.MassTransit
{
    public delegate void ServiceBusConfiguration(ServiceBusConfigurator configure);
}