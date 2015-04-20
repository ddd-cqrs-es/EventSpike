using MassTransit;
using Topshelf;

namespace EventSpike.ApprovalService
{
    internal class ApprovalServiceControl :
        ServiceControl
    {
        private readonly IServiceBus _bus;

        public ApprovalServiceControl(IServiceBus bus)
        {
            _bus = bus;
        }

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