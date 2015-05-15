using EventSpike.Common;
using MassTransit;
using Topshelf;

namespace EventSpike.Approval.Service
{
    internal class ApprovalServiceControl :
        ServiceControl
    {
        private readonly IServiceBus _bus;
        private readonly ISystemInitializer _initializer;

        public ApprovalServiceControl(IServiceBus bus, ISystemInitializer initializer)
        {
            _bus = bus;
            _initializer = initializer;
        }

        public bool Start(HostControl hostControl)
        {
            _initializer.Initialize();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}