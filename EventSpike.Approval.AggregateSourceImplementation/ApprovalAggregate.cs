using System;
using EventSpike.Approval.AggregateSourceIntegration;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.Approval.AggregateSourceImplementation
{
    public class ApprovalAggregate : AggregateRootEntity
    {
        public static readonly Func<ApprovalAggregate> Factory = () => new ApprovalAggregate();
        private Guid _id;

        private ApprovalAggregate()
        {
            Register<ApprovalInitiated>(@event => _id = @event.Id);
        }

        public ApprovalAggregate(Guid id, string title, string description) :
            this()
        {
            ApplyChange(new ApprovalInitiated
            {
                Id = id,
                Title = title,
                Description = description
            });
        }

        public void ReviseDescription(string description)
        {
            ApplyChange(new ApprovalDescriptionRevised
            {
                Id = _id,
                Description = description
            });
        }

        public void MarkAccepted(string referenceNumber)
        {
            ApplyChange(new ApprovalAccepted
            {
                Id = _id,
                ReferenceNumber = referenceNumber
            });
        }

        public void MarkPartiallyAccepted(string referenceNumber)
        {
            ApplyChange(new ApprovalPartiallyAccepted
            {
                Id = _id,
                ReferenceNumber = referenceNumber
            });
        }

        public void MarkDenied(string referenceNumber, string denialReasonCode)
        {
            ApplyChange(new ApprovalDenied
            {
                Id = _id,
                DenialReason = denialReasonCode,
                ReferenceNumber = referenceNumber
            });
        }

        public void Cancel(string cancellationResonCode)
        {
            ApplyChange(new ApprovalCancelled
            {
                Id = _id,
                CancellationReason = cancellationResonCode
            });
        }
    }
}
