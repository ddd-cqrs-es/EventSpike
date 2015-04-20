using MassTransit.BusConfigurators;

namespace EventSpike.Common.MassTransit
{
    public delegate void ServiceBusConfiguration(ServiceBusConfigurator configure);
}