using CommonDomain.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.CommonDomain;
using MassTransit;

namespace EventSpike.ApprovalService
{
    internal class ApprovalCommandMassTransitConsumer :
        Consumes<CommandEnvelope<InitiateApproval>>.All,
        Consumes<CommandEnvelope<MarkApprovalAccepted>>.All,
        Consumes<CommandEnvelope<MarkApprovalPartiallyAccepted>>.All,
        Consumes<CommandEnvelope<MarkApprovalDenied>>.All,
        Consumes<CommandEnvelope<MarkApprovalCancelled>>.All
    {
        private readonly ITenantProvider<IRepository> _repositoryProvider;

        public ApprovalCommandMassTransitConsumer(ITenantProvider<IRepository> repositoryProvider)
        {
            _repositoryProvider = repositoryProvider;
        }

        public void Consume(CommandEnvelope<InitiateApproval> message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = new ApprovalAggregate(message.Body.Id, message.Body.Title, message.Body.Description);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }

        public void Consume(CommandEnvelope<MarkApprovalAccepted> message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Body.Id);

            approval.MarkAccepted(message.Body.ReferenceNumber);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }

        public void Consume(CommandEnvelope<MarkApprovalCancelled> message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Body.Id);

            approval.Cancel(message.Body.CancellationReason);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {UserId = message.UserId}));
        }

        public void Consume(CommandEnvelope<MarkApprovalDenied> message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Body.Id);

            approval.MarkDenied(message.Body.ReferenceNumber, message.Body.DenialReason);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {UserId = message.UserId}));
        }

        public void Consume(CommandEnvelope<MarkApprovalPartiallyAccepted> message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Body.Id);

            approval.MarkPartiallyAccepted(message.Body.ReferenceNumber);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }
    }
}