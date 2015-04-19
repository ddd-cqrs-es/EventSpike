using MassTransit;
using NEventStore.Spike.Common;
using Topshelf;

namespace NEventStore.Spike.ProjectionService
{
    public class ProjectionServiceControl : ServiceControl
    {
        private readonly IServiceBus _bus;
        private readonly EventSubscriptionBootstrapper _subscriptionBootstrapper;

        public ProjectionServiceControl(IServiceBus bus, EventSubscriptionBootstrapper subscriptionBootstrapper)
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
