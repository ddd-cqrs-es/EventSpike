using System;

namespace EventSpike.ApprovalMessages.Commands
{
    public class MarkApprovalCancelled
    {
        public Guid Id { get; set; }
        public string CancellationReason { get; set; }
    }
}