using System;
using CommonDomain.Core;
using NEventStore.Spike.Common.ApprovalEvents;

namespace NEventStore.Spike.ApprovalService
{
    internal class ApprovalAggregate :
        AggregateBase
    {
        private bool _isDescriptionLocked;

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
            if (_isDescriptionLocked)
            {
                throw new ApprovalModifiedWhenLocked("Description can not be revised when the approval is locked");
            }

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
            _isDescriptionLocked = false;
        }

        private void Apply(ApprovalAccepted @event)
        {
            _isDescriptionLocked = true;
        }

        private void Apply(ApprovalPartiallyAccepted @event)
        {
            _isDescriptionLocked = true;
        }

        private void Apply(ApprovalDenied @event)
        {
            _isDescriptionLocked = false;
        }

        private void Apply(ApprovalCancelled @event)
        {
            _isDescriptionLocked = true;
        }
    }
}