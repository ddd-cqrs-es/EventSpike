using MassTransit.BusConfigurators;

namespace EventSpike.Common.Autofac
{
    public delegate void ServiceBusConfigurationDelegate(ServiceBusConfigurator configure);
}