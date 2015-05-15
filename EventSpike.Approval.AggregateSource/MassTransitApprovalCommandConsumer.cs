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
        private readonly Repository<ApprovalAggregate> _repository;
        private readonly NEventStoreUnitOfWorkCommitter _committer;

        public MassTransitApprovalCommandConsumer(Repository<ApprovalAggregate> repository, NEventStoreUnitOfWorkCommitter committer)
        {
            _repository = repository;
            _committer = committer;
        }

        public void Consume(IConsumeContext<InitiateApproval> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = new ApprovalAggregate(context.Message.Id, context.Message.Title, context.Message.Description);

            _repository.Add(context.Message.Id.ToString(), approval);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalAccepted> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.Get(context.Message.Id.ToString());

            approval.MarkAccepted(context.Message.ReferenceNumber);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalPartiallyAccepted> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.Get(context.Message.Id.ToString());

            approval.MarkPartiallyAccepted(context.Message.ReferenceNumber);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalDenied> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.Get(context.Message.Id.ToString());

            approval.MarkDenied(context.Message.ReferenceNumber, context.Message.DenialReason);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }

        public void Consume(IConsumeContext<MarkApprovalCancelled> context)
        {
            var causationId = context.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.Get(context.Message.Id.ToString());

            approval.Cancel(context.Message.CancellationReason);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(context.Headers));
        }
    }
}
