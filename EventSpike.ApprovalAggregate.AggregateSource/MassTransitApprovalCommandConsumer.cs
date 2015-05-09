using AggregateSource.NEventStore;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;
using MassTransit;

namespace EventSpike.ApprovalAggregate.AggregateSource
{
    public class MassTransitApprovalCommandConsumer :
        Consumes<InitiateApproval>.Context,
        Consumes<MarkApprovalAccepted>.Context,
        Consumes<MarkApprovalPartiallyAccepted>.Context,
        Consumes<MarkApprovalDenied>.Context,
        Consumes<MarkApprovalCancelled>.Context
    {
        private readonly ITenantProvider<Repository<ApprovalAggregate>> _repositoryProvider;
        private readonly ITenantProvider<NEventStoreUnitOfWorkCommitter> _committer;

        public MassTransitApprovalCommandConsumer(ITenantProvider<Repository<ApprovalAggregate>> repositoryProvider, ITenantProvider<NEventStoreUnitOfWorkCommitter> committer)
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
            throw new System.NotImplementedException();
        }

        public void Consume(IConsumeContext<MarkApprovalPartiallyAccepted> context)
        {
            throw new System.NotImplementedException();
        }

        public void Consume(IConsumeContext<MarkApprovalDenied> context)
        {
            throw new System.NotImplementedException();
        }

        public void Consume(IConsumeContext<MarkApprovalCancelled> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
