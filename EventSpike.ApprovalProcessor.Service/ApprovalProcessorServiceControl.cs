using System.Collections.Generic;
using EventSpike.Common;
using MassTransit;
using Topshelf;

namespace EventSpike.ApprovalProcessor.Service
{
    internal class ApprovalProcessorServiceControl :
        ServiceControl
    {
        private readonly IServiceBus _bus;
        private readonly IEnumerable<INeedInitialization> _initializers;

        public ApprovalProcessorServiceControl(IServiceBus bus, IEnumerable<INeedInitialization> initializers)
        {
            _bus = bus;
            _initializers = initializers;
        }

        public bool Start(HostControl hostControl)
        {
            foreach (var initializer in _initializers)
            {
                initializer.Initialize();
            }

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            return true;
        }
    }
}
