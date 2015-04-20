using Automatonymous;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.ApprovalEvents;
using NEventStore.Spike.Common.CommonDomain;

namespace NEventStore.Spike.ApprovalProcessorService
{
    public class ApprovalProcessEventHandler :
        IHandle<IEnvelope<ApprovalInitiated>>
    {
        private readonly ITenantProvider<IApprovalProcessorRepository> _repositoryProvider;

        public ApprovalProcessEventHandler(ITenantProvider<IApprovalProcessorRepository> repositoryProvider)
        {
            _repositoryProvider = repositoryProvider;
        }

        public void Handle(IEnvelope<ApprovalInitiated> message)
        {
            var tenantId = message.Headers.Retrieve<SystemHeaders>().TenantId;

            var processor = _repositoryProvider
                .Get(tenantId)
                .GetProcessorById(message.Body.Id);

            processor.Raise(processor.
        }
    }
}
