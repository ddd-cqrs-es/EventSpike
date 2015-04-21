using System;

namespace EventSpike.Common.ApprovalCommands
{
    public class MarkApprovalCancelled
    {
        public Guid Id { get; set; }
        public string CancellationReason { get; set; }
    }
}