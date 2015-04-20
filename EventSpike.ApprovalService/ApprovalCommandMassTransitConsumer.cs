using CommonDomain.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using EventSpike.Common.CommonDomain;
using MassTransit;

namespace EventSpike.ApprovalService
{
    internal class ApprovalCommandMassTransitConsumer :
        Consumes<InitiateApproval>.All,
        Consumes<MarkApprovalAccepted>.All,
        Consumes<MarkApprovalPartiallyAccepted>.All,
        Consumes<MarkApprovalDenied>.All,
        Consumes<MarkApprovalCancelled>.All
    {
        private readonly ITenantProvider<IRepository> _repositoryProvider;

        public ApprovalCommandMassTransitConsumer(ITenantProvider<IRepository> repositoryProvider)
        {
            _repositoryProvider = repositoryProvider;
        }

        public void Consume(InitiateApproval message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = new ApprovalAggregate(message.Id, message.Title, message.Description);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }

        public void Consume(MarkApprovalAccepted message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Id);

            approval.MarkAccepted(message.ReferenceNumber);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }

        public void Consume(MarkApprovalCancelled message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Id);

            approval.Cancel(message.CancellationReason);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {UserId = message.UserId}));
        }

        public void Consume(MarkApprovalDenied message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Id);

            approval.MarkDenied(message.ReferenceNumber, message.DenialReason);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {UserId = message.UserId}));
        }

        public void Consume(MarkApprovalPartiallyAccepted message)
        {
            var repository = _repositoryProvider.Get(message.TenantId);
            var approval = repository.GetById<ApprovalAggregate>(message.Id);

            approval.MarkPartiallyAccepted(message.ReferenceNumber);

            repository.Save(approval, message.CausationId, headers => headers.Store(new SystemHeaders {TenantId = message.TenantId, UserId = message.UserId}));
        }
    }
}