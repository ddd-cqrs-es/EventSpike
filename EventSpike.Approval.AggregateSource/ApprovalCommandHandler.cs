using AggregateSource.NEventStore;
using EventSpike.Approval.AggregateSource.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;

namespace EventSpike.Approval.AggregateSource
{
    public class ApprovalCommandHandler : IHandler
    {
        private readonly Repository<ApprovalAggregate> _repository;
        private readonly NEventStoreUnitOfWorkCommitter _committer;

        public ApprovalCommandHandler(Repository<ApprovalAggregate> repository, NEventStoreUnitOfWorkCommitter committer)
        {
            _repository = repository;
            _committer = committer;
        }

        public void Handle(Envelope<InitiateApproval> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = new ApprovalAggregate(envelope.Message.Id, envelope.Message.Title, envelope.Message.Description);

            _repository.Add(envelope.Message.Id.ToString(), approval);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(envelope.Headers));
        }

        public void Handle(Envelope<MarkApprovalAccepted> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.Get(envelope.Message.Id.ToString());

            approval.MarkAccepted(envelope.Message.ReferenceNumber);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(envelope.Headers));
        }

        public void Handle(Envelope<MarkApprovalPartiallyAccepted> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.Get(envelope.Message.Id.ToString());

            approval.MarkPartiallyAccepted(envelope.Message.ReferenceNumber);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(envelope.Headers));
        }

        public void Handle(Envelope<MarkApprovalDenied> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.Get(envelope.Message.Id.ToString());

            approval.MarkDenied(envelope.Message.ReferenceNumber, envelope.Message.DenialReason);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(envelope.Headers));
        }

        public void Handle(Envelope<MarkApprovalCancelled> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.Get(envelope.Message.Id.ToString());

            approval.Cancel(envelope.Message.CancellationReason);

            _committer.Commit(_repository.UnitOfWork, causationId, headers => headers.CopyFrom(envelope.Headers));
        }
    }
}
