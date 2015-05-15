using AggregateSource.NEventStore;
using EventSpike.Approval.AggregateSource.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using MassTransit;

namespace EventSpike.Approval.AggregateSource
{
    public class MassTransitApprovalCommandConsumer :
        Consumes<InitiateApproval>.Context,
        Consumes<MarkApprovalAccepted>.Context,
        Consumes<MarkApprovalPartiallyAccepted>.Context,
        Consumes<MarkApprovalDenied>.Context,
        Consumes<MarkApprovalCancelled>.Context
    {
        private readonly IProvideForTenant<Repository<ApprovalAggregate>> _repositoryProvider;
        private readonly IProvideForTenant<NEventStoreUnitOfWorkCommitter> _committer;

        public MassTransitApprovalCommandConsumer(IProvideForTenant<Repository<ApprovalAggregate>> repositoryProvider, IProvideForTenant<NEventStoreUnitOfWorkCommitter> committer)
        {
            _repositoryProvider = repositoryProvider;
            _committer = committer;
        }

        public void Consume(IConsumeContext<InitiateApproval> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);

            var approval = new ApprovalAggregate(context.Message.Id, context.Message.Title, context.Message.Description);

            repository.Add(context.Message.Id.ToString(), approval);

            _committer.Get(tenantId).Commit(repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalAccepted> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = repository.Get(context.Message.Id.ToString());

            approval.MarkAccepted(context.Message.ReferenceNumber);

            _committer.Get(tenantId).Commit(repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalPartiallyAccepted> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = repository.Get(context.Message.Id.ToString());

            approval.MarkPartiallyAccepted(context.Message.ReferenceNumber);

            _committer.Get(tenantId).Commit(repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalDenied> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = repository.Get(context.Message.Id.ToString());

            approval.MarkDenied(context.Message.ReferenceNumber, context.Message.DenialReason);

            _committer.Get(tenantId).Commit(repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalCancelled> context)
        {
            var tenantId = context.Headers[Constants.TenantIdKey];
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var repository = _repositoryProvider.Get(tenantId);
            var approval = repository.Get(context.Message.Id.ToString());

            approval.Cancel(context.Message.CancellationReason);

            _committer.Get(tenantId).Commit(repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }
    }
}
