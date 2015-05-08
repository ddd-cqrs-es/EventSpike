using System;
using CommonDomain.Core;
using EventSpike.Common.ApprovalEvents;

namespace EventSpike.ApprovalAggregate.CommonDomain
{
    internal class ApprovalAggregate :
        AggregateBase
    {
        private ApprovalAggregate(Guid id)
        {
            Id = id;
        }

        public ApprovalAggregate(Guid id, string title, string description) : this(id)
        {
            RaiseEvent(new ApprovalInitiated
            {
                Id = id,
                Title = title,
                Description = description
            });
        }

        public void ReviseDescription(string description)
        {
            RaiseEvent(new ApprovalDescriptionRevised
            {
                Id = Id,
                Description = description
            });
        }

        public void MarkAccepted(string referenceNumber)
        {
            RaiseEvent(new ApprovalAccepted
            {
                Id = Id,
                ReferenceNumber = referenceNumber
            });
        }

        public void MarkPartiallyAccepted(string referenceNumber)
        {
            RaiseEvent(new ApprovalPartiallyAccepted
            {
                Id = Id,
                ReferenceNumber = referenceNumber
            });
        }

        public void MarkDenied(string referenceNumber, string denialReasonCode)
        {
            RaiseEvent(new ApprovalDenied
            {
                Id = Id,
                DenialReason = denialReasonCode,
                ReferenceNumber = referenceNumber
            });
        }

        public void Cancel(string cancellationResonCode)
        {
            RaiseEvent(new ApprovalCancelled
            {
                Id = Id,
                CancellationReason = cancellationResonCode
            });
        }

        private void Apply(ApprovalInitiated @event)
        {
        }

        private void Apply(ApprovalAccepted @event)
        {
        }

        private void Apply(ApprovalPartiallyAccepted @event)
        {
        }

        private void Apply(ApprovalDenied @event)
        {
        }

        private void Apply(ApprovalCancelled @event)
        {
        }
    }
}