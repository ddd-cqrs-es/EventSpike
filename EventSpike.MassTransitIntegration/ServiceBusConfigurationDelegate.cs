using MassTransit.BusConfigurators;

namespace EventSpike.MassTransitIntegration
{
    public delegate void ServiceBusConfigurationDelegate(ServiceBusConfigurator configure);
}