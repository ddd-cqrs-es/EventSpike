using System;

namespace EventSpike.Approval.Messages.Commands
{
    public class MarkApprovalDenied
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string DenialReason { get; set; }
    }
}