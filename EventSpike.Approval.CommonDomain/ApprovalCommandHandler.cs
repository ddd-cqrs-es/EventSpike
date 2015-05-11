using CommonDomain.Persistence;
using EventSpike.Common;
using EventSpike.Common.ApprovalCommands;

namespace EventSpike.Approval.CommonDomain
{
    public class ApprovalCommandHandler :
        IHandle<Envelope<InitiateApproval>>,
        IHandle<Envelope<MarkApprovalAccepted>>,
        IHandle<Envelope<MarkApprovalPartiallyAccepted>>,
        IHandle<Envelope<MarkApprovalDenied>>,
        IHandle<Envelope<MarkApprovalCancelled>>
    {
        private readonly IRepository _repository;

        public ApprovalCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Envelope<InitiateApproval> envelope)
        {
            var approval = new ApprovalAggregate(envelope.Message.Id, envelope.Message.Title, envelope.Message.Description);
            
            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _repository.Save(approval, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }

        public void Handle(Envelope<MarkApprovalAccepted> envelope)
        {
            var approval = _repository.GetById<ApprovalAggregate>(envelope.Message.Id);

            approval.MarkAccepted(envelope.Message.ReferenceNumber);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _repository.Save(approval, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }

        public void Handle(Envelope<MarkApprovalPartiallyAccepted> envelope)
        {
            var approval = _repository.GetById<ApprovalAggregate>(envelope.Message.Id);

            approval.MarkPartiallyAccepted(envelope.Message.ReferenceNumber);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _repository.Save(approval, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }

        public void Handle(Envelope<MarkApprovalDenied> envelope)
        {
            var approval = _repository.GetById<ApprovalAggregate>(envelope.Message.Id);

            approval.MarkDenied(envelope.Message.ReferenceNumber, envelope.Message.DenialReason);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _repository.Save(approval, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }

        public void Handle(Envelope<MarkApprovalCancelled> envelope)
        {
            var approval = _repository.GetById<ApprovalAggregate>(envelope.Message.Id);

            approval.Cancel(envelope.Message.CancellationReason);

            var commitId = envelope.Headers[Constants.CausationIdKey].ToGuid();
            _repository.Save(approval, commitId, headers => headers.CopyFrom(envelope.Headers.ToDictionary()));
        }
    }
}