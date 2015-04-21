using Automatonymous;
using EventSpike.Common;
using EventSpike.Common.ApprovalEvents;
using EventSpike.Common.CommonDomain;
using MassTransit;

namespace EventSpike.ApprovalProcessorService.Automatonymous
{
    public class ApprovalProcessEventHandler :
        IHandler
    {
        private readonly IServiceBus _bus;
        private readonly ITenantProvider<IApprovalProcessorRepository> _repositoryProvider;

        public ApprovalProcessEventHandler(IServiceBus bus, ITenantProvider<IApprovalProcessorRepository> repositoryProvider)
        {
            _bus = bus;
            _repositoryProvider = repositoryProvider;
        }

        public void Handle(IEnvelope<ApprovalInitiated> message)
        {
            
            var tenantId = message.Headers.Retrieve<SystemHeaders>().TenantId;

            var processorInstance = _repositoryProvider
                .Get(tenantId)
                .GetProcessorById(message.Body.Id);

            var processor = new ApprovalProcessor {Bus = _bus};

            processor.RaiseEvent(processorInstance, eventIs => eventIs.Initiated, message);
        }

        public void Handle(IEnvelope<ApprovalAccepted> message)
        {
            var tenantId = message.Headers.Retrieve<SystemHeaders>().TenantId;

            var processorInstance = _repositoryProvider
                .Get(tenantId)
                .GetProcessorById(message.Body.Id);

            var processor = new ApprovalProcessor { Bus = _bus };

            processor.RaiseEvent(processorInstance, eventIs => eventIs.Accepted, message);
        }
    }
}
