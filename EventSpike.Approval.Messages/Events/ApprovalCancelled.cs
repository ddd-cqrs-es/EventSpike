using System;

namespace EventSpike.Approval.Messages.Events
{
    public class ApprovalCancelled
    {
        public Guid Id { get; set; }
        public string CancellationReason { get; set; }
    }
}