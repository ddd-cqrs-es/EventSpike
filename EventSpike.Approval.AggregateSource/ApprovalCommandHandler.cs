using AggregateSource.NEventStore;
using EventSpike.Approval.AggregateSource.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;

namespace EventSpike.Approval.AggregateSource
{
    public class ApprovalCommandHandler :
        IHandle<Envelope<InitiateApproval>>,
        IHandle<Envelope<MarkApprovalAccepted>>,
        IHandle<Envelope<MarkApprovalPartiallyAccepted>>,
        IHandle<Envelope<MarkApprovalDenied>>,
        IHandle<Envelope<MarkApprovalCancelled>>
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
            var approval = new ApprovalAggregate(envelope.Message.Id, envelope.Message.Title, envelope.Message.Description);

            _repository.Add(envelope.Message.Id.ToString(), approval);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _committer.Commit(_repository.UnitOfWork, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }

        public void Handle(Envelope<MarkApprovalAccepted> envelope)
        {
            var approval = _repository.Get(envelope.Message.Id.ToString());

            approval.MarkAccepted(envelope.Message.ReferenceNumber);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _committer.Commit(this._repository.UnitOfWork, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }

        public void Handle(Envelope<MarkApprovalPartiallyAccepted> envelope)
        {
            var approval = _repository.Get(envelope.Message.Id.ToString());

            approval.MarkPartiallyAccepted(envelope.Message.ReferenceNumber);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _committer.Commit(_repository.UnitOfWork, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }

        public void Handle(Envelope<MarkApprovalDenied> envelope)
        {
            var approval = _repository.Get(envelope.Message.Id.ToString());

            approval.MarkDenied(envelope.Message.ReferenceNumber, envelope.Message.DenialReason);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _committer.Commit(_repository.UnitOfWork, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }

        public void Handle(Envelope<MarkApprovalCancelled> envelope)
        {
            var approval = _repository.Get(envelope.Message.Id.ToString());

            approval.Cancel(envelope.Message.CancellationReason);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _committer.Commit(_repository.UnitOfWork, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }
    }
}