using CommonDomain.Persistence;
using EventSpike.ApprovalMessages.Commands;
using EventSpike.Common;

namespace EventSpike.Approval.CommonDomainImplementation
{
    public class ApprovalCommandHandler : IHandler
    {
        private readonly IRepository _repository;

        public ApprovalCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Envelope<InitiateApproval> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = new ApprovalAggregate(envelope.Message.Id, envelope.Message.Title, envelope.Message.Description);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(envelope.Headers));
        }

        public void Handle(Envelope<MarkApprovalAccepted> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.GetById<ApprovalAggregate>(envelope.Message.Id);

            approval.MarkAccepted(envelope.Message.ReferenceNumber);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(envelope.Headers));
        }

        public void Handle(Envelope<MarkApprovalCancelled> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.GetById<ApprovalAggregate>(envelope.Message.Id);

            approval.Cancel(envelope.Message.CancellationReason);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(envelope.Headers));
        }

        public void Handle(Envelope<MarkApprovalDenied> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.GetById<ApprovalAggregate>(envelope.Message.Id);

            approval.MarkDenied(envelope.Message.ReferenceNumber, envelope.Message.DenialReason);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(envelope.Headers));
        }

        public void Handle(Envelope<MarkApprovalPartiallyAccepted> envelope)
        {
            var causationId = envelope.Headers[Constants.CausationIdKey].ToGuid();

            var approval = _repository.GetById<ApprovalAggregate>(envelope.Message.Id);

            approval.MarkPartiallyAccepted(envelope.Message.ReferenceNumber);

            _repository.Save(approval, causationId, headers => headers.CopyFrom(envelope.Headers));
        }
    }
}