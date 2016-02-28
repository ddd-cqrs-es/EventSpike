using System;

namespace EventSpike.Approval.Messages.Commands
{
    public class MarkApprovalCancelled
    {
        public Guid Id { get; set; }
        public string CancellationReason { get; set; }
    }
}