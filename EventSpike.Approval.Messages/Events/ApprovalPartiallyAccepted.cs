using System;

namespace EventSpike.Approval.Messages.Events
{
    public class ApprovalPartiallyAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}