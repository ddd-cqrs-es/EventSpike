using EventSpike.Common.EventSubscription;
using MassTransit;
using Topshelf;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class ApprovalProcessorServiceControl :
        ServiceControl
    {
        private readonly IServiceBus _bus;
        private readonly EventSubscriptionBootstrapper _subscriptionBootstrapper;

        public ApprovalProcessorServiceControl(IServiceBus bus, EventSubscriptionBootstrapper subscriptionBootstrapper)
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
