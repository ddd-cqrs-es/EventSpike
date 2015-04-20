using System;

namespace EventSpike.Common.ApprovalEvents
{
    public class ApprovalCancelled
    {
        public Guid Id { get; set; }
        public string CancellationReason { get; set; }
    }
}