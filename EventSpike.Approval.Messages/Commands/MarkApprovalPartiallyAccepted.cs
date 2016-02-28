using System;

namespace EventSpike.Approval.Messages.Commands
{
    public class MarkApprovalPartiallyAccepted
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
    }
}