using CommonDomain.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalEvents;
using EventSpike.Common.CommonDomain;

namespace EventSpike.ApprovalProcessor.CommonDomain
{
    public class ApprovalProcessEventHandler :
        IEventHandler
    {
        private readonly ITenantProvider<ISagaRepository> _sagaRepositoryProvider;
        
        public static readonly string UserId = string.Format("#{0}#", typeof(ApprovalProcessor).Name);

        public ApprovalProcessEventHandler(ITenantProvider<ISagaRepository> sagaRepositoryProvider)
        {
            _sagaRepositoryProvider = sagaRepositoryProvider;
        }

        public void Handle(IEnvelope<ApprovalInitiated> message)
        {
            var tenantId = message.Headers.Retrieve<SystemHeaders>().TenantId;

            var repository = _sagaRepositoryProvider.Get(tenantId);

            var sagaId = ApprovalProcessorConstants.DeterministicGuid.Create(message.Body.Id.ToByteArray()).ToString();

            var saga = repository.GetById<ApprovalProcessor>(sagaId);

            saga.Transition(message.Body);

            var causationId = message.Headers.Retrieve<ContextHeaders>().CausationId;
            var commitId = ApprovalProcessorConstants.DeterministicGuid.Create(causationId.ToByteArray());

            repository.Save(saga, commitId, headers => headers.Store(new SystemHeaders {TenantId = tenantId, UserId = UserId}));
        }

        public void Handle(IEnvelope<ApprovalAccepted> message)
        {
            var tenantId = message.Headers.Retrieve<SystemHeaders>().TenantId;

            var repository = _sagaRepositoryProvider.Get(tenantId);

            var saga = repository.GetById<ApprovalProcessor>(message.Body.Id);

            saga.Transition(message.Body);

            var causationId = message.Headers.Retrieve<ContextHeaders>().CausationId;
            var commitId = ApprovalProcessorConstants.DeterministicGuid.Create(causationId.ToByteArray());

            repository.Save(saga, commitId, headers => headers.Store(new SystemHeaders { TenantId = tenantId, UserId = UserId }));
        }
    }
}