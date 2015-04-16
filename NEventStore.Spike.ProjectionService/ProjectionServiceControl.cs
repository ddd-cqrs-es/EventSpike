using NEventStore.Spike.Common;
using Topshelf;

namespace NEventStore.Spike.ProjectionService
{
    public class ProjectionServiceControl : ServiceControl
    {
        private readonly EventSubscriptionBootstrapper _subscriptionBootstrapper;

        public ProjectionServiceControl(EventSubscriptionBootstrapper subscriptionBootstrapper)
        {
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
