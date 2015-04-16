using CommonDomain.Persistence;
using MassTransit;
using NEventStore.Spike.Common;
using NEventStore.Spike.Common.ApprovalCommands;

namespace NEventStore.Spike.ApprovalService
{
    internal class ApprovalCommandConsumer :
        Consumes<InitiateApproval>.All,
        Consumes<MarkApprovalAccepted>.All
    {
        private readonly TenantProvider<IRepository> _repositoryProvider;

        public ApprovalCommandConsumer(TenantProvider<IRepository> repositoryProvider)
        {
            _repositoryProvider = repositoryProvider;
        }

        public void Consume(InitiateApproval message)
        {
            var repository = _repositoryProvider(message.TenantId);
            var approval = new ApprovalAggregate(message.Id, message.Title, message.Description);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }

        public void Consume(MarkApprovalAccepted message)
        {
            var repository = _repositoryProvider(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Id);

            approval.MarkAccepted(message.ReferenceNumber);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }

        public void Consume(MarkApprovalPartiallyAccepted message)
        {
            var repository = _repositoryProvider(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Id);

            approval.MarkPartiallyAccepted(message.ReferenceNumber);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }

        public void Consume(MarkApprovalDenied message)
        {
            var repository = _repositoryProvider(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Id);

            approval.MarkDenied(message.ReferenceNumber, message.DenialReason);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {UserId = message.UserId}));
        }

        public void Consume(MarkApprovalCancelled message)
        {
            var repository = _repositoryProvider(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Id);

            approval.Cancel(message.CancellationReason);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {UserId = message.UserId}));
        }
    }
}
