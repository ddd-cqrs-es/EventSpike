using EventSpike.Runtime;
using Logary;
using MassTransit;
using Topshelf;

namespace EventSpike.Approval.ServiceHost
{
    internal class ApprovalServiceControl :
        ServiceControl
    {
        private readonly LogManager _logManager;
        private readonly IServiceBus _bus;
        private readonly ISystemInitializer _initializer;

        public ApprovalServiceControl(LogManager logManager, IServiceBus bus, ISystemInitializer initializer)
        {
            _logManager = logManager;
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
            _logManager.Dispose();
            _bus.Dispose();

            return true;
        }
    }
}