using System;

namespace EventSpike.ApprovalMessages.Events
{
    public class ApprovalCancelled
    {
        public Guid Id { get; set; }
        public string CancellationReason { get; set; }
    }
}