using MassTransit.BusConfigurators;

namespace EventSpike.Common.Registries
{
    public delegate void ServiceBusConfigurationDelegate(ServiceBusConfigurator configure);
}