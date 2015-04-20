using EventSpike.Common.EventSubscription;
using MassTransit;
using Topshelf;

namespace EventSpike.ConsoleOutputService
{
    public class ConsoleServiceControl : ServiceControl
    {
        private readonly IServiceBus _bus;
        private readonly EventSubscriptionBootstrapper _subscriptionBootstrapper;

        public ConsoleServiceControl(IServiceBus bus, EventSubscriptionBootstrapper subscriptionBootstrapper)
        {
            _bus = bus;
            _subscriptionBootstrapper = subscriptionBootstrapper;
        }

        public bool Start(HostControl hostControl)
        {
            _subscriptionBootstrapper.ResumeSubscriptions();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}