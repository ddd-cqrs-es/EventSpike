using Topshelf;

namespace NEventStore.Spike.ProjectionService
{
    public class ProjectionServiceControl : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}
