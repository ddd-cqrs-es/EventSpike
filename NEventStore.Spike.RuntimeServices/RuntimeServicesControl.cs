using MassTransit.Services.Subscriptions.Server;
using Topshelf;

namespace NEventStore.Spike.RuntimeServices
{
    internal class RuntimeServicesControl : ServiceControl
    {
        private readonly SubscriptionService _subscriptionService;

        public RuntimeServicesControl(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public bool Start(HostControl hostControl)
        {
            _subscriptionService.Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _subscriptionService.Stop();

            return true;
        }
    }
}